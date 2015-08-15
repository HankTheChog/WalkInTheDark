using UnityEngine;
using System.Collections;
using System;

public class Maping : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// building an easy map - only up and right turns. EasyMap gets a two dimensional array  
	// and builds an easy map in it. the number 0 represents a wall (or solid matter)
	// and the number 1 represents a room (the path, a hallway, air, what ever you want) 
	public static void EasyMap(int[,] map)
	{
		System.Random rng = new System.Random();

		// setting the whole map to 0, everything is a wall
		for (int i = 0; i < map.GetLength(0); i++) {
			for (int j = 0; j < map.GetLength(1); j++) {
				map[i,j]=0;
			}
		}
		//[0,0] is the starting point and so it always equals to 1. [0,0] represents the
		// right bottom point of the map
		map[0, 0] = 1;

		//indexes to the last tile that was changed
		int CurrentRow = 0; 
		int CurrentCol = 0;

		// the pathway can be as long as the sum of the number of columns
		// and rows in the array minus 1
		for (int i = 0; i < (map.GetLength(0) + map.GetLength(1)); i++)
		{
			if (CurrentCol < map.GetLength(1) - 1 && CurrentRow < map.GetLength(0) - 1)
			{
				//getting a random number (0 or 1), if you get 0 the tile on the right 
				//becomes a room, if you get 1 the above becomes a room. the indexes 
				//changes acording to the last tile that was modified
				int num = rng.Next(0, 2);
				if (num == 0)
				{
					CurrentCol++;
					map[CurrentRow, CurrentCol] = 1;
				}
				if (num == 1)
				{
					CurrentRow++;
					map[CurrentRow, CurrentCol] = 1;
				}
			}
		}
		
	}
}
