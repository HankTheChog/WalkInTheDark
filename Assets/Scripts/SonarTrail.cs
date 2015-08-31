using UnityEngine;
using System.Collections;

public class SonarTrail : MonoBehaviour {
	float expirationTime;

	void Start() {
		expirationTime = Time.time + Game.instance.sonarDuration;
	}

	// Update is called once per frame
	void Update () {
		if (Time.time >= expirationTime)
			Destroy(gameObject);	
	}
}
