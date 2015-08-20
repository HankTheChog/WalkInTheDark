using UnityEngine;
using System.Collections;

public class LerpMap : MonoBehaviour {
	bool functional = false;
	public Light lightEx;
	public SpriteRenderer colorEx;
	public float OverTimeEx;
	public float StartAtEx;
	public float StopAtEx;
	public bool TickForLight;
	public bool TickForColor;

	// Use this for initialization
	void Start () {
		// Maybe start lerping?
		SetDimming(functional); // note that this will set monitored attributes to StartAtEx, even if !active
	}

	public bool lightsOn {
		get {
			return !functional;
		}
	}

	IEnumerator LerpLight(Light lighty,float overTime,float StartAt, float StopAt){
		float startTime = Time.time;
		float place = 0;
		float sum = StartAt - StopAt;
		while (place<1f) {
			place=((Time.time-startTime) / overTime);
			float intes = sum*place;
			intes =sum-intes;
			lighty.intensity=intes;
			yield return null;
		}
	}

	IEnumerator LerpColor(SpriteRenderer spr,float overTime,float StartAt, float StopAt){
		Color clone = spr.color;
		float starttime=Time.time;
		float place = 0;
		//StartAt = (1 / 255) * StartAt;<------ if value is not calculated before entering use this
		//StopAt = (1 / 255) * StopAt;<------ if value is not calculated before entering use this
		float sum = StartAt - StopAt;
		while (place<1f) {
			place=((Time.time-starttime)/overTime);
			float current = sum*place;
			current = sum-current;
			Debug.Log(current);
			clone.a=current;
			spr.color = clone;
			yield return null;
		}
	}

	/// <param name="start">If true, restarts the script, else halts it and returns monitored attributes (light intensity and/or renderer color)
	/// to initial value (StartAtEx).</param>
	public void SetDimming(bool start) {
		if (!start) {
			// Stop lerping and reset lerped attributes to starting values
			StopAllCoroutines();
			if (TickForColor) {} // TODO: implement for color, presently only works for light
			if (TickForLight) {
				lightEx.intensity = StartAtEx;
			}
		} else {
			if (TickForColor) {
				StartCoroutine(LerpColor(colorEx,OverTimeEx,StartAtEx,StopAtEx));
			}
			if (TickForLight) {
				StartCoroutine(LerpLight(lightEx,OverTimeEx,StartAtEx,StopAtEx));
			}
		}
		
		functional = start;
	}
}
