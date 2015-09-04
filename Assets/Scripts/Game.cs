using UnityEngine;
using System.Collections;

/*public struct Rules {
	bool wallsLethal;
}*/ // Oh, wait, Unity's inspector can't display structs etc, because it is bad.

public class Game : MonoBehaviour {
	private static Game zInstance = null;

	private float bestWinTime = -1;

	GameObject failText;

	/// <summary>For toggling map visibility</summary>
	LerpMap lightController;

	/// <summary>For crude, ugly bounds-checking on player movement.</summary>
	[HideInInspector]
	public Vector2 mapBoundsMin;
	
	/// <summary>For crude, ugly bounds-checking on player movement.</summary>
	[HideInInspector]
	public Vector2 mapBoundsMax;

	[HideInInspector]
	public Assets.BasicMovement player;

	/// <summary>Duration of sonar trail, in seconds.</summary>
	public float sonarDuration;

	[SerializeField]
	Transform sonarPrefab;

	[SerializeField]
	int sonarRange;

	public bool wallsLethal;

	/// <summary>Tiles presumed square, this is their side length (y-extent is disregarded). Descriptive, does not presently resize anything.</summary>
	float zTileSize = 1.0f;

	[SerializeField]
	float timerDuration;

	float timeStarted = -1;

	/// <summary>Time between a restart and the point at which the map fade BEGINS (assuming the player does not preempt it by taking a step).</summary>
	[SerializeField]
	float timeUntilInitialFade;
	
	private GameObject tombstone;

	/// <summary>Number of times the torch was used *this run*.</summary>
	private int torchesUsed = 0;

	/// <summary>If false, count up, else count down.</summary>
	bool useTimer = false;

	GameObject winText;

	/// <summary>Singleton pattern.</summary>
	public static Game instance {
		get {
			return zInstance;
		}		
	}

	void Start() {
		B.Assert(zInstance == null); 
		zInstance = this;

		// Find objects in scene, init references
		failText = GameObject.Find("FailText");
		lightController = FindObjectOfType<LerpMap>(); // assume unique
		player = GameObject.Find("Player").GetComponent<Assets.BasicMovement>();
		tombstone = GameObject.Find("Tombstone");
		winText = GameObject.Find("WinText");
		B.Assert(player != null && lightController != null && tombstone != null);
		// ^Could use Resources.Load() here instead, but since both the player and tombstone are unique, we might as well just preplace them in the scene

		StartClock();
	}

	/// <summary>Returns true if the given position is out of bounds or inside a wall</summary>
	/// <note>Blocked positions don't necessarily block movement, they might just kill the player character (if wallsLethal).</note>
	public bool Blocked(Vector2 position) {
		var p = position - mapBoundsMin;
		var b = mapBoundsMax - mapBoundsMin;

		return p.x < 0 || p.y < 0 || p.x > b.x || p.y > b.y || Physics2D.Raycast(position, Vector2.zero, 0, LayerMask.GetMask("Obstacle"));	
	}

	public bool Blocked(Vector3 position) {
		return Blocked(new Vector2(position.x, position.y));
	}

	public void Die() {
		// Place tombstone at last tile visited, make it visible
		tombstone.transform.position = player.currentTile; // poor encapsulation
		tombstone.GetComponent<MeshRenderer>().enabled = true;

		// Display message and restart
		failText.SetActive(true);
		winText.SetActive(false);
		Restart();
	}

	void NextStage() {
		// Generate new map, resete everything. This is a bit kludgy.
		FindObjectOfType<Maping>().GenerateMap();

		bestWinTime = -1;
		tombstone.GetComponent<MeshRenderer>().enabled = false;

		failText.SetActive(false);
		winText.SetActive(false);
		Restart();
	}

	void OnGUI() {
		if (timeElapsed >= 0) {
			// Show time
			float timeToDisplay = timeElapsed;
			if (useTimer)
				timeToDisplay = timerDuration - timeToDisplay;
			GUILayout.Label("DO NOT PANIC: " + (timeToDisplay).ToString("0.00"));
		} else
			GUILayout.Label("Be ready for it");

		if (bestWinTime >= 0) {
			var timeToDisplay = bestWinTime;
			if (useTimer)
				timeToDisplay = timerDuration - bestWinTime;
			GUILayout.Label("Best win time: " + timeToDisplay.ToString("0.00"));
		}

		GUILayout.Label("Torches used: " + torchesUsed);
	}

	public void OnStartMove() {
		if (lightController.lightsOn) {
			// Dim the lights
			lightController.SetDimming(true);
		}
	}

	public void Restart() {
		StartClock();
		torchesUsed = 0;
		player.Reset();
		lightController.SetDimming(false);

		Assets.Scripts.Enemy.Instance.Reset();
	}
	
	public void StartClock() {
		timeStarted = Time.time;
	}

	public float tileSize {
		get {
			return zTileSize;
		}
	}

	/// <summary>In seconds, since last restart.</summary>
	float timeElapsed {
		get {
			if (timeStarted >= 0)
				return Time.time - timeStarted;
			return -1;
		}
	}

	void Update() {
		// Are we out of time?
		if (useTimer && timeElapsed > timerDuration) {
			Die();
			return;
		}

		// Are the lights still on? Should they be?
		if (lightController.lightsOn && timeStarted >= 0 && Time.time - timeStarted >= timeUntilInitialFade)
			lightController.SetDimming(true);

		// If L key pressed while dark, use torch
		if (Input.GetKeyDown(KeyCode.L) && !lightController.lightsOn) {
			++torchesUsed;
			lightController.SetDimming(false);	
		}

		// S - sonar
		if (Input.GetKeyDown(KeyCode.S)) {
			++torchesUsed;
			var sonar = Instantiate(sonarPrefab, player.transform.position, Quaternion.identity) as Transform;
			sonar.GetComponent<Sonar>().range = sonarRange;
		}

		// R - generate new map
		if (Input.GetKeyDown(KeyCode.R)) {
			NextStage();
		}

		// T - toggle timer
		if (Input.GetKeyDown(KeyCode.T)) {
			useTimer = !useTimer;
		}

		// V - toggle player visibility
		if (Input.GetKeyDown(KeyCode.V)) {
			player.isLit = !player.isLit;
		}
	}

	public void Win() {
		bestWinTime = timeElapsed;
		failText.SetActive(false);
		winText.SetActive(true);
		Restart();
	}
}
