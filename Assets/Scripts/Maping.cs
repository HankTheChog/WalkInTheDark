﻿using UnityEngine;
using System.Collections;
using System;

public enum TileType {
	Floor,
	Wall
}

/// <summary>Fully describes a hypothetical map. Does not imply the actual Unity prefabs have been instantiated yet.</summary>
/// Maps are assumed to be rectangular, their extent is the size of the tiles array.
public class MapDescriptor {
	public TileType[,] tiles;

	public int startX;
	public int startY;

	public int endX;
	public int endY;
}

public class Maping : MonoBehaviour {
	[SerializeField]
	Transform edgePrefab;

	[SerializeField]
	Transform floorPrefab;

	GameObject goal;

	[SerializeField]
	Transform gridPrefab;
		
	public int numTilesX;
	public int numTilesY;

	[SerializeField]
	Transform wallPrefab;

	
	void Start () {
		// Find goal in scene
		goal = GameObject.Find("Goal");
		// Create map
		GenerateMap();
	}

	/// <summary>Generates a map and returns its descriptor.</summary>
	public MapDescriptor GenerateMap() { 
		// Destroy any extant map
		var mapContainer = GameObject.Find("Map").transform;
		foreach (Transform child in mapContainer)
			Destroy(child.gameObject); // Might be faster to just destroy and recreate the empty mapContainer

		// Create new map
		var result = EasyMap(numTilesX, numTilesY);
		InstantiateMap(result);
		return result;
	}

	static void MoveUp(int col, TileType[,] map)
	{

		int row = map.GetLength(0)-1;
		
		while (map[row-1, col] != TileType.Floor)
		{
			row--;
		}
		map[row, col] = TileType.Floor;
		
		for (int i = col+1; i < map.GetLength(1); i++)
		{
			for (int j = map.GetLength(0)-1; j > 0; j--)
			{
				if (map[j-1, i] == TileType.Floor)
				{
					map[j, i] = TileType.Floor;
					map[j - 1, i] = TileType.Wall;
				}
			}
		}
	}

	static void MoveRight(int row, TileType[,] map)
	{
		int col = map.GetLength(1) - 1;
		while (map[row, col - 1]!=TileType.Floor)
		{
			col--;
		}
		map[row,col]=TileType.Floor;
		
		for (int i = row+1; i < map.GetLength(0); i++)
		{
			for (int j = map.GetLength(1)-1; j > 0; j--)
			{
				if (map[i, j - 1] == TileType.Floor)
				{
					map[i, j] = TileType.Floor;
					map[i,j - 1] = TileType.Wall;
				}
			}
		}
	}
	/// <summary>
	/// finds the greatest common divisor of two integers.
	/// helpfull for better map generating
	/// </summary>
	static int FindGCD(int x, int y)
	{
		int Max = Math.Max(x, y);
		int Min = Math.Min(x, y);
		int Remainder = Max;
		while (Min!=0)
		{
			while (Remainder >= Min)
			{
				Remainder =Remainder- Min;
			}
			Max = Min;
			Min = Remainder;
			Remainder = Max;
		}
		return Remainder;
	}

	// building an easy map - only up and right turns. EasyMap gets a two dimensional array  
	// and builds an easy map in it. the number 0 represents a wall (or solid matter)
	// and the number 1 represents a room (the path, a hallway, air, what ever you want) 
	static MapDescriptor EasyMap(int sizeX, int sizeY)
	{
		System.Random rng = new System.Random();
		var tiles = new TileType[sizeX, sizeY];

		//finding the greatest common divisor of the lengths of the rectangle.
		// this gives a better way of randomizing tiles, instead in a ratio of 1:1
		// the ratio depends on the size of the rectangle (for a 10 by 20 rectangle
		// the randomizing ratio for up and right will be 1:2)
		int gcd = FindGCD(tiles.GetLength(1), tiles.GetLength(0));
		int ChanceNum = (tiles.GetLength(1) / gcd) + (tiles.GetLength(0) / gcd);

		// setting the whole tiles to 0, everything is a wall
		for (int i = 0; i < tiles.GetLength(0); i++) {
			for (int j = 0; j < tiles.GetLength(1); j++) {
				tiles[i,j] = TileType.Wall;
			}
		}
		//[0,0] is the starting point and so it always equals to 1. [0,0] represents the
		// right bottom point of the map
		tiles[0, 0] = TileType.Floor;

		//indexes to the last tile that was changed
		int CurrentRow = 0; 
		int CurrentCol = 0;
		int num;
		// the pathway can be as long as the sum of the number of columns
		// and rows in the array minus 1
		for (int i = 0; i < (tiles.GetLength(0) + tiles.GetLength(1)); i++)
		{
			if (CurrentCol < tiles.GetLength(1) - 1 && CurrentRow < tiles.GetLength(0) - 1)
			{
				//getting a random number between 0 and the chance num, so the odds 
				//of getting an up or right turn will be adjusted by the size of the rectangle
				 num = rng.Next(0, ChanceNum);


				if (num < Math.Min((tiles.GetLength(1) / gcd), (tiles.GetLength(0) / gcd)))
				{
					if(tiles.GetLength(1)>tiles.GetLength(0)){
						CurrentRow++;
						tiles[CurrentRow, CurrentCol] = TileType.Floor;
					
					}
					else
					{
						CurrentCol++;
						tiles[CurrentRow, CurrentCol] = TileType.Floor;
					}
				}
				else{

					if(tiles.GetLength(1)>tiles.GetLength(0))
					{
						CurrentCol++;
						tiles[CurrentRow, CurrentCol] = TileType.Floor;
						
					}else
					{
						CurrentRow++;
						tiles[CurrentRow, CurrentCol] = TileType.Floor;
					}
				}
			}
		}

		while (CurrentRow != tiles.GetLength(0) - 1)
		{
			
			num = rng.Next(0, tiles.GetLength(1));
			MoveUp(num, tiles);
			CurrentRow++;
		}
		while (CurrentCol != tiles.GetLength(1) - 1)
		{
			num = rng.Next(0, tiles.GetLength(0));
			MoveRight(num, tiles);
			CurrentCol++;
		}

		var result = new MapDescriptor();
		result.tiles = tiles;
		// Simple paths always start at the bottom left, end at the top right
		result.startX = 0;
		result.startY = 0;
		result.endX = sizeX;
		result.endY = sizeY;

		return result;
	}

	/// <summary>Given a position and a map descriptor, creates that map at said position.</summary>
	void InstantiateMap(MapDescriptor descriptor, Vector3 position = default(Vector3)) {
		var tiles = descriptor.tiles;
		var tileSize = Game.instance.tileSize;

		// Find empty gameobject to keep map in, to keep our scene hierarchy in order
		var mapContainer = GameObject.Find("Map").transform;

		position -= new Vector3(tiles.GetLength(0), tiles.GetLength(1)) * tileSize * 0.5f;

		// TEMP: also make a note of the map bounds, for player movement
		Game.instance.mapBoundsMin = position;
		Game.instance.mapBoundsMax = position + new Vector3(tiles.GetLength(0) - 1, tiles.GetLength(1) - 1) * tileSize;

		for (var x = 0; x < tiles.GetLength(0); ++x) {
			for (var y = 0; y < tiles.GetLength(1); ++y) {
				// Place tile
				var tileToPlace = floorPrefab;
				var tilePosition = position + new Vector3(x, y) * tileSize;			

				// Wait, is the tile a wall? 
				if (tiles[x, y] == TileType.Wall) {
					// Yes. Find edges, place edge sprites on them
					tileToPlace = wallPrefab;
					if ((x > 0) && (tiles[x - 1, y] == TileType.Floor)) {
						var edge = Instantiate(edgePrefab, tilePosition, Quaternion.Euler(0, 0, 180)) as Transform;
						edge.parent = mapContainer;
					}
					if ((y > 0) && (tiles[x, y - 1] == TileType.Floor)) {
						var edge = Instantiate(edgePrefab, tilePosition, Quaternion.Euler(0, 0, 270)) as Transform;
						edge.parent = mapContainer;
					}
					if ((x < tiles.GetLength(0) - 1) && (tiles[x + 1, y] == TileType.Floor)) {
						var edge = Instantiate(edgePrefab, tilePosition, Quaternion.identity) as Transform;
						edge.parent = mapContainer;
					}
					if ((y < tiles.GetLength(1) - 1) && (tiles[x, y + 1] == TileType.Floor)) {
						var edge = Instantiate(edgePrefab, tilePosition, Quaternion.Euler(0, 0, 90)) as Transform;
						edge.parent = mapContainer;
					}
				}

				var tile = Instantiate(tileToPlace, tilePosition, Quaternion.identity) as Transform;
				tile.parent = mapContainer;

				// Also place visualization of grid cell
				var cell = Instantiate(gridPrefab, tilePosition, Quaternion.identity) as Transform;
				cell.parent = mapContainer;
			}
		}

		// Place goal at end
		goal.transform.position = position + new Vector3(tiles.GetLength(0) - 1, tiles.GetLength(1) - 1) * tileSize;

		// TEMP: place the player at the start position. This is kinda bad encapsulation, and needlessly constrains SEO
		var playerPos = Game.instance.player.transform.position;
		playerPos = position + new Vector3(descriptor.startX * tileSize, descriptor.startY * tileSize);
		Game.instance.player.transform.position = playerPos;
		Game.instance.player.startPosition = playerPos;
		Game.instance.player.Reset(); // why is this necessary?

        Assets.Scripts.Enemy.Instance.Initiate(playerPos);
	}
}
