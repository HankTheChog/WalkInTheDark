using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {
	private static Game zInstance = null;
	
	/// <summary>For toggling map visibility</summary>
	LerpMap lightController;
	public Assets.BasicMovement player;
	/// <summary>Tiles presumed square, this is their side length (y-extent is disregarded). Descriptive, does not presently resize anything.</summary>
	float zTileSize = 1.0f;
	float timeStarted = -1;

	/// <summary>Time between a restart and the point at which the map fade BEGINS (assuming the player does not preempt it by taking a step).</summary>
	[SerializeField]
	float timeUntilInitialFade;

	/// <summary>
	/// Singleton pattern.
	/// </summary>
	public static Game instance {
		get {
			return zInstance;
		}		
	}

	void Start() {
		B.Assert(zInstance == null); 
		zInstance = this;

		// Find objects in scene, init references
		lightController = FindObjectOfType<LerpMap>(); // assume unique
		player = GameObject.Find("Player").GetComponent<Assets.BasicMovement>();
		B.Assert(player != null && lightController != null);
	}

	void OnGUI() {
		GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "DO NOT PANIC: " + ((int)timeElapsed).ToString());
	}

	public void OnStartMove() {
		if (lightController.lightsOn) {
			// Dim the lights
			lightController.SetDimming(true);
		}
	}

	public void Restart() {
		StartClock();
		player.Reset();
		lightController.SetDimming(false);
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
			return Time.time - timeStarted;
		}
	}

	void Update() {
		// Are the lights still on? Should they be?
		if (lightController.lightsOn && timeStarted >= 0 && Time.time - timeStarted >= timeUntilInitialFade)
			lightController.SetDimming(true);
	}
}
