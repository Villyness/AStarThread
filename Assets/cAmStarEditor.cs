using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(cAmStar))]
public class cAmStarEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
        
		cAmStar myScript = (cAmStar)target;
		if(GUILayout.Button("Random start/target"))
		{
			myScript.RandomlyPositionStartAndTarget();
		}
		if(GUILayout.Button("Find path"))
		{
			myScript.ClearMap();
			myScript.FindPath();
		}
	}
}