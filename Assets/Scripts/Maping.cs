using UnityEngine;
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
	public int startZ;

	public int endX;
	public int endZ;
}

public class Maping : MonoBehaviour {
	[SerializeField]
	Transform floorPrefab;

	GameObject goal;

	[SerializeField]
	Transform wallPrefab;
		
	[SerializeField]
	int numTilesX;
	[SerializeField]
	int numTilesZ;

	
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
		var result = EasyMap(numTilesX, numTilesZ);
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
	static MapDescriptor EasyMap(int sizeX, int sizeZ)
	{
		System.Random rng = new System.Random();
		var tiles = new TileType[sizeX, sizeZ];

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
		result.startZ = 0;
		result.endX = sizeX;
		result.endZ = sizeZ;

		return result;
	}

	/// <summary>Given a position and a map descriptor, creates that map at said position.</summary>
	void InstantiateMap(MapDescriptor descriptor, Vector3 position = default(Vector3)) {
		var tiles = descriptor.tiles;
		var tileSize = Game.instance.tileSize;

		// Find empty gameobject to keep map in, to keep our scene hierarchy in order
		var mapContainer = GameObject.Find("Map").transform;

		position -= new Vector3(tiles.GetLength(0), 0, tiles.GetLength(1)) * tileSize * 0.5f;

		for (var x = 0; x < tiles.GetLength(0); ++x) {
			for (var z = 0; z < tiles.GetLength(1); ++z) {
				var tileToPlace = wallPrefab;
				if (tiles[x, z] == TileType.Floor)
					tileToPlace = floorPrefab;

				var tilePosition = position + new Vector3(x, 0, z) * tileSize;			

				var tile = Instantiate(tileToPlace, tilePosition, Quaternion.identity) as Transform;
				tile.parent = mapContainer;
			}
		}

		// Place goal at end
		goal.transform.position = position + new Vector3(tiles.GetLength(0) - 1, 0, tiles.GetLength(1) - 1) * tileSize;

		// TEMP: place the player at the start position. This is kinda bad encapsulation, and needlessly constrains SEO
		var playerPos = Game.instance.player.transform.position;
		playerPos = position + new Vector3(descriptor.startX * tileSize, playerPos.y, descriptor.startZ * tileSize);
		Game.instance.player.transform.position = playerPos;
	}
}
