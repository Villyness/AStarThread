using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
	public Node[,]    grid;
	public GameObject gridPrefab;
	public GameObject gridPrefabBlocked;
	public Vector2Int size;

	// Start is called before the first frame update
	void Awake()
	{
		SpawnGrid();
	}


	public Vector2Int FindUnblockedSpace()
	{
		Vector2 randomPosition      = new Vector2(0, 0);
		bool    foundUnblockedSpace = false;
		while (foundUnblockedSpace == false)
		{
			int x = Random.Range(0, size.x - 1);
			int y = Random.Range(0, size.y - 1);
			randomPosition = new Vector2(x, y);
			if (grid[x, y].isBlocked == false)
			{
				foundUnblockedSpace = true;
				return new Vector2Int(x, y);
			}
		}

		return new Vector2Int(-1, -1);
	}

	private void SpawnGrid()
	{
		// Spawn grid
		grid = new Node[size.x, size.y];

		for (int x = 0; x < size.x; x++)
		{
			for (int y = 0; y < size.y; y++)
			{
				grid[x, y]          = new Node();
				grid[x, y].position = new Vector2Int(x, y);
//grid[x, y].isBlocked = Random.Range(0, 10) > 6; // HACK random map
				grid[x, y].isBlocked = Mathf.PerlinNoise(x / 5f, y / 5f) > 0.5f; // HACK random map
				GameObject o;
				if (grid[x, y].isBlocked)
				{
					o = Instantiate(gridPrefabBlocked, new Vector3(x, 0, y), Quaternion.identity);
					o.GetComponentInChildren<Renderer>().material.color = Color.red;
				}
				else
				{
					o = Instantiate(gridPrefab, new Vector3(x, 0, y), Quaternion.identity);
					o.GetComponentInChildren<Renderer>().material.color = Color.green;
					// HACK debug
				}

				grid[x, y].debugGO = o;
			}
		}
	}

//	private void OnDrawGizmos()
//	{
//		for (int x = 0; x < size.x; x++)
//		{
//			for (int y = 0; y < size.y; y++)
//			{
//				if (grid != null)
//				{
//					if (grid[x, y].isBlocked)
//					{
//						Gizmos.color = Color.red;
//						Gizmos.DrawCube(new Vector3(x, 0, y), Vector3.one);
//					}
//				}
//			}
//		}


//		// Scan the real world starting at 0,0,0 (to be able to place the grid add transform.position)
//		for (int x = 0; x < size.x; x++)
//		{
//			for (int y = 0; y < size.y; y++)
//			{
//				if (Physics.CheckBox(transform.position + new Vector3(x, 0, y), new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity))
//				{
//					// Something is there
//					grid[x, y].isBlocked = true;
//					Gizmos.color = Color.red;
//					Gizmos.DrawCube(transform.position + new Vector3(x, 0, y), Vector3.one);
//				}
//			}
//		}
//	}
}