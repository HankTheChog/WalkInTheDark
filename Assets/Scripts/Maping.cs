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
}

public class Maping : MonoBehaviour {

	public Transform floorPrefab;
	public Transform wallPrefab;

	
	void Start () {
		var map = EasyMap(15, 10);

		InstantiateMap(map);
	}

	// building an easy map - only up and right turns. EasyMap gets a two dimensional array  
	// and builds an easy map in it. the number 0 represents a wall (or solid matter)
	// and the number 1 represents a room (the path, a hallway, air, what ever you want) 
	static MapDescriptor EasyMap(int sizeX, int sizeZ)
	{
		System.Random rng = new System.Random();

		var tiles = new TileType[sizeX, sizeZ];

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

		// the pathway can be as long as the sum of the number of columns
		// and rows in the array minus 1
		for (int i = 0; i < (tiles.GetLength(0) + tiles.GetLength(1)); i++)
		{
			if (CurrentCol < tiles.GetLength(1) - 1 && CurrentRow < tiles.GetLength(0) - 1)
			{
				//getting a random number (0 or 1), if you get 0 the tile on the right 
				//becomes a room, if you get 1 the above becomes a room. the indexes 
				//changes acording to the last tile that was modified
				int num = rng.Next(0, 2);
				if (num == 0)
				{
					CurrentCol++;
					tiles[CurrentRow, CurrentCol] = TileType.Floor;
				}
				if (num == 1)
				{
					CurrentRow++;
					tiles[CurrentRow, CurrentCol] = TileType.Floor;
				}
			}
		}

		var result = new MapDescriptor();
		result.tiles = tiles;
		result.startX = 0;
		result.startZ = 0;

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

		// TEMP: place the player at the start position. This is kinda bad encapsulation, and needlessly constrains SEO
		var playerPos = Game.instance.player.transform.position;
		playerPos = position + new Vector3(descriptor.startX * tileSize, playerPos.y, descriptor.startZ * tileSize);
		Game.instance.player.transform.position = playerPos;
	}
}
