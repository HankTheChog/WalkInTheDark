using UnityEngine;
using System.Collections;

public class Sonar : PathFollower {
	/// <summary>Range in tiles.</summary>
	public int range;

	[SerializeField]
	Transform trailPrefab;

	protected override void OnMove()
	{
		--range;
		if (range <= 0)
			Destroy(gameObject);
		else {
			Instantiate(trailPrefab, transform.position, Quaternion.identity);
		}
	}
}
