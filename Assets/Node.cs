using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Node
{
	public bool isBlocked = false;
	public Node parent;
	public int  gCost = 0;
	public int  hCost = 0;
	public int  fCost = 0;

	public Vector2 position;

	// HACK debug
	public GameObject debugGO;

	public void Reset()
	{
		parent                                                    = null;
		gCost                                                     = 0;
		hCost                                                     = 0;
		fCost                                                     = 0;
		if (debugGO != null)
		{
			if (debugGO.GetComponentInChildren<TextMesh>() != null)
			{
				debugGO.GetComponentInChildren<TextMesh>().text = "";				
			}
			if(isBlocked)
				debugGO.GetComponentInChildren<Renderer>().material.color = Color.red;
			else
			{
				debugGO.GetComponentInChildren<Renderer>().material.color = Color.white;
			}
		}
	}
}