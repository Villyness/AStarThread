using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class cAmStar : MonoBehaviour
{
	public Map map;

	public List<Node> open;
	public List<Node> closed;
	public List<Node> finalPath;

	public Node current;


	public GameObject startPrefab;
	public GameObject targetPrefab;
	public GameObject pathCubePrefab;
	public Vector2    start;
	public Vector2    target;
	public float      visualiseSpeed = 0.1f;

	GameObject targetIndicator;
	GameObject startIndicator;

	public event Action OnFoundPath;
	public event Action OnBlockedPath;

	void Start()
	{
		startIndicator  = Instantiate(startPrefab);
		targetIndicator = Instantiate(targetPrefab);

		RandomlyPositionStartAndTarget();

//        DemoMode();
//        OnFoundPath += DemoMode;
	}

	public void DemoMode()
	{
		RandomlyPositionStartAndTarget();
		FindPath();
	}

	public void ClearMap()
	{
		finalPath.Clear();
		open.Clear();
		closed.Clear();

		foreach (Node node in map.grid)
		{
			if (node.debugGO != null)
			{
				node.Reset();
			}
		}
	}

	public List<Node> FindPath()
	{
//        StartCoroutine(FindPathCoroutine());
		return FindPathCoroutine();
	}

	public List<Node> FindPath(Vector2 _start, Vector2 _target)
	{
		start  = _start;
		target = _target;

		startIndicator.transform.position  = new Vector3(start.x, 0, start.y);
		targetIndicator.transform.position = new Vector3(target.x, 0, target.y);

		return FindPath();
	}

//	private IEnumerator FindPathCoroutine()
	private List<Node> FindPathCoroutine()
	{
		// Debug
		ClearMap();

		float xCheck = 0;
		float yCheck = 0;
		int   fCost;
		int   gCost;
		int   hCost;
		Node  nodeToCheck;

		current = map.grid[(int) start.x, (int) start.y];
		open.Add(current); // Initial starting point

		// Loop until end found
		while (open.Count > 0)
		{
			current = FindLowestFCost();
			// HACK TODO DEBUG
			current.debugGO.GetComponentInChildren<Renderer>().material.color = Color.green;

			// Node is closed
			open.Remove(current);

			// TODO: Check shouldn't need the contains check
			if (!closed.Contains(current))
				closed.Add(current);


			if (CheckReachedTarget())
			{
//                yield return new WaitForSeconds(2f);
				OnFoundPath?.Invoke();
				
				//                yield break;
				return finalPath;
			}


			// Neighbours recalc
			for (int x = -1; x < 2; x++)
			{
				for (int y = -1; y < 2; y++)
				{
					// Same as current so bail
					if (x == 0 && y == 0)
						continue;

					xCheck = current.position.x + x;
					yCheck = current.position.y + y;

					// Bail if out of bounds or the current node, or in the closed list
					if (xCheck < 0 || yCheck < 0 || xCheck >= map.size.x || yCheck >= map.size.y)
						continue;

					nodeToCheck = map.grid[(int) xCheck, (int) yCheck];
					// Bail if node used or blocked
					if (closed.Contains(nodeToCheck) || nodeToCheck.isBlocked)
						continue;

					// Note: Multiply by ten to maintain ints for distances
					hCost = (int) (10 * Vector2.Distance(
														 nodeToCheck.position,
														 target));
					gCost = current.gCost + (int) (10f * Vector2.Distance(
																		  current.position,
																		  nodeToCheck.position));

					// fCost
					fCost = hCost + gCost;

					// Bail if the existing fCost is lower
					if (nodeToCheck.fCost != 0 && fCost > nodeToCheck.fCost)
						continue;

					// All good, so record new values (don't do it WHILE you're calculating the f,g,h costs because they rely on previous results)
					nodeToCheck.hCost = hCost;
					nodeToCheck.gCost = gCost;
					nodeToCheck.fCost = fCost;

					// Debug
					if (nodeToCheck.debugGO.GetComponentInChildren<TextMesh>() != null)
						nodeToCheck.debugGO.GetComponentInChildren<TextMesh>().text =
							nodeToCheck.gCost + ":" + nodeToCheck.hCost + "\n" + nodeToCheck.fCost;

					nodeToCheck.parent = current;

					Debug.DrawLine(new Vector3(nodeToCheck.position.x, 0, nodeToCheck.position.y),
								   new Vector3(nodeToCheck.position.x, 10f, nodeToCheck.position.y), Color.magenta,
								   0.1f, false);

					// TODO: Shouldn't need the contains check
					if (!open.Contains(nodeToCheck))
						open.Add(nodeToCheck);

//                    if (visualiseSpeed > 0) yield return new WaitForSeconds(visualiseSpeed / 10f);
				}
			}

			// HACK TODO DEBUG
			current.debugGO.GetComponentInChildren<Renderer>().material.color = Color.blue;

//            if (visualiseSpeed > 0) yield return new WaitForSeconds(visualiseSpeed);
		}

//        yield return new WaitForSeconds(2f);
		OnBlockedPath?.Invoke();
		return null;
	}

	private bool CheckReachedTarget()
	{
		// Reached end
		if (current.position == target)
		{
			while (current.parent != null)
			{
				finalPath.Add(current);
				current.debugGO.GetComponentInChildren<Renderer>().material.color = Color.green;
				current                                                           = current.parent;
//				if (visualiseSpeed > 0) yield return new WaitForSeconds(visualiseSpeed);
			}

			// Because it get added from the END back to the start
			finalPath.Reverse();

			return true;
		}

		return false;
	}

	private Node FindLowestFCost()
	{
//        int lowest = open.Min(Node => Node.fCost);

		// Find next lowest fCost
		int  lowestFCost     = int.MaxValue;
		Node lowestFCostNode = null;

		foreach (Node node in open)
		{
			if (node.fCost < lowestFCost)
			{
				lowestFCost     = node.fCost;
				lowestFCostNode = node;
			}
		}

		return lowestFCostNode;
	}

	public void RandomlyPositionStartAndTarget()
	{
		start                              = map.FindUnblockedSpace();
		startIndicator.transform.position  = new Vector3(start.x, 0, start.y);
		target                             = map.FindUnblockedSpace();
		targetIndicator.transform.position = new Vector3(target.x, 0, target.y);
	}


//    public void Update()
//    {
//        // Don't continuously update if we want to visualise manually
//        if (visualiseSpeed > 0)
//            return;
//
//        start.x = (int) startIndicator.transform.position.x;
//        start.y = (int) startIndicator.transform.position.z;
//
//        target.x = (int) targetIndicator.transform.position.x;
//        target.y = (int) targetIndicator.transform.position.z;
//
//        ClearMap();
////		RandomlyPositionStartAndTarget();
//        FindPath();
//    }


//    private void OnDrawGizmos()
//    {
//        foreach (Node node in open)
//        {
//            Gizmos.color = Color.red;
//            Gizmos.DrawCube( new Vector3(node.position.x, -0.45f, node.position.x), Vector3.one);
//        }
//    }
//        for (int x = 0; x < map.size.x; x++)
//        {
//            for (int y = 0; y < map.size.y; y++)
//            {
//                if (map.grid != null)
//                {
//                    if (map.grid[x, y].isBlocked)
//                    {
//                        Gizmos.color = Color.red;
//                        Gizmos.DrawCube(new Vector3(x, -0.45f, y), Vector3.one);
//                    }
//
//                    if (open.Contains(map.grid[x, y]))
//                    {
//                        Gizmos.color = Color.green;
//                        Gizmos.DrawCube(new Vector3(x, -0.45f, y), Vector3.one);
//                    }
//
//                    if (closed.Contains(map.grid[x, y]))
//                    {
//                        Gizmos.color = Color.gray;
//                        Gizmos.DrawCube(new Vector3(x, -0.45f, y), Vector3.one);
//                    }
//                }
//            }
//        }


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
//    }
}