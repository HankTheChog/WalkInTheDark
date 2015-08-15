using UnityEngine;
using System.Collections;

public class LerpMap : MonoBehaviour {
	public Light lightEx;
	public SpriteRenderer colorEx;
	public float OverTimeEx;
	public float StartAtEx;
	public float StopAtEx;
	public bool TickForLight;
	public bool TickForColor;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (TickForColor) {
			TickForColor=false;
			StartCoroutine(LerpColor(colorEx,OverTimeEx,StartAtEx,StopAtEx));
		}
		if (TickForLight) {
			TickForLight=false;
			StartCoroutine(LerpLight(lightEx,OverTimeEx,StartAtEx,StopAtEx));
		}
	

	}

	public void LerpLightV(Light lighty,float overTime,float StartAt, float StopAt){
		StartCoroutine(LerpLight(lighty,overTime,StartAt,StopAt));
	}
	public void LerpAlphaV(SpriteRenderer spr,float overTime,float StartAt, float StopAt){
		StartCoroutine(LerpColor(spr,overTime,StartAt,StopAt));
	}

	IEnumerator LerpLight(Light lighty,float overTime,float StartAt, float StopAt){


		float starttime=Time.time;
		float place = 0;
		float sum = StartAt - StopAt;
		while (place<1f) {
			place=((Time.time-starttime)/overTime);
			float intes = sum*place;
			intes =sum-intes;
			lighty.intensity=intes;
			yield return null;
		}
		DoneLerpLight ();
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
		DoneLerpAlpha ();
	}
	void DoneLerpLight(){

	}
	void DoneLerpAlpha(){

	}
}
