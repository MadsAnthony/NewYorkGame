using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rotorz.ReorderableList;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(LevelDatabase))]
public class LevelDatabaseInspector : Editor {
	public override void OnInspectorGUI()
	{
		ReorderableListGUI.Title("Levels");
		ReorderableListGUI.ListField(serializedObject.FindProperty("levels"));

		serializedObject.ApplyModifiedProperties();
	}
}
#endif