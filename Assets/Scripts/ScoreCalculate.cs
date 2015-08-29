using UnityEngine;
using System.Collections;

public class ScoreCalculate : MonoBehaviour {
	public float silver=110;
	public float gold=100;
	public float platinum=90;
	float startTime;
	float endTime;
	float torcheUse;
	public float scaleValueTime=1f;
	public float ScaleValueTorche=5f;
	float gameDuration;


	public void CallOnUseTorch(){
		//call this void when using the torch
		torcheUse++;
	}
	public void CallOnStart(){
		// call this void on game start
		startTime = Time.time;
	}
	public void CallOnEnd(){
		//call this void when game is over
		endTime = Time.time;

		gameDuration = endTime - startTime;

		//calculating the score
		CalculateScore ();
	}

	void CalculateScore(){
		//calculating the score


		//change the scale value to icrease/decrease the % from the total value
		float timeValue = gameDuration *= scaleValueTime;
		//change the scale value to icrease/decrease the % from the total value
		float torcheValue = torcheUse *= ScaleValueTorche;

		// this is the total value
		float sum = timeValue + torcheValue;

		//setting the medal based on score
		trophyType (sum);
	}
	void trophyType(float sum){
		//setting the medal based on score
		string medal = "";
		if (sum <= platinum) {
			medal = "platinum";
		}
		if (sum <= gold&&sum>platinum) {
			medal = "gold";
		}
		if (sum <= silver&&sum>gold) {
			medal = "silver";
		}
		if (sum>silver) {
			medal = "bronze";
		}

		//doing what ever needed to be done at the end
		end (medal);
	}
	void end(string medal){
		switch (medal){
		case "platinum":

			//do stuff here

			break;
		case "gold":
			
			//do stuff here

			break;
		case "silver":
			
			//do stuff here

			break;
		case "bronze":
			
			//do stuff here

			break;
		}
	}
}
