using UnityEngine;
using System.Collections;

public class Analytics : MonoBehaviour {
	float gameStart;
	float gameEnd;
	int LightUse;


	public void useLight(){
		//call this void when using the tourch
		LightUse++;
	}


	public void SetStartTime(){
		//call this void when the game session start
		gameStart = Time.time;
	}


	public void SetEndTime(){
		//call this void when game is over
		gameEnd = Time.time;
		saveDate ();// saving the data collected during the game
	}
	void saveDate(){
		// saving the data collected during the game
		PlayerPrefs.SetFloat ("gameStart", gameStart);
		PlayerPrefs.SetFloat ("gameEnd", gameEnd);
		PlayerPrefs.SetInt ("LightUse", LightUse);
	}




	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////


	//showing the data

	
	public void showData(){
		//call this void to show the data from the last session
		float startT = PlayerPrefs.GetFloat ("gameStart");
		float endT = PlayerPrefs.GetFloat ("gameEnd");
		int Luse = PlayerPrefs.GetInt ("LightUse");

		//duration of last game as float
		float duration = endT - startT;

		//duration of last game with 2 numbers after the whole number aka 34.45
		float readableDuration = duration * 100;
		int tempdur = (int)readableDuration;
		readableDuration = tempdur / 100;

		//duration of last game as Int
		int durationInt = (int)duration;


		//pick what ever kind of way you want to show time and set it equal to timey below

		float timey = readableDuration;



		//string for readable data - i can add time stamp and a method to search all saved strings for deeper use
		string lastGame = "last game session was " + timey + " seconds and the Light was used " + Luse + " times";
		// show the data to console
		Debug.Log (lastGame);
	}


}
