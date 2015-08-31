using UnityEngine;
using System.Collections;

/// <summary>For entities that follow the path through the maze (like the enemy)</summary>
public class PathFollower : MonoBehaviour {	
	[SerializeField]
	private float tileMoveTime;

	void Start() {
		StartCoroutine(Move());
	}

	/// <summary>
	/// This method checks if there isn't obstacle on right.
	/// If yes, Enemy will move forward.
	/// If not, Enemy will move right.
	/// </summary>
	private bool CanMoveRight()
	{
		return !Game.instance.Blocked(transform.position + new Vector3(1, 0, 0) * Game.instance.tileSize);
	}
	
	protected IEnumerator Move()
	{
		while (true)
		{
			yield return new WaitForSeconds(tileMoveTime);
			
			if (CanMoveRight())
				transform.Translate(new Vector2(1, 0));
			else
				transform.Translate(Vector2.up);  

			OnMove();
		}
	}

	virtual protected void OnMove() { }
}
