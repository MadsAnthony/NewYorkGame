using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(PieceDatabase))]
public class PieceDatabaseInspector : Editor {
	Vector2 scrollPos;

	public override void OnInspectorGUI()
	{
		PieceDatabase myTarget = (PieceDatabase)target;

		string[] pieceTypes = Enum.GetNames (typeof(PieceType));
		foreach(var pieceTypeStr in pieceTypes) {
			PieceType pieceType = (PieceType)Enum.Parse (typeof(PieceType), pieceTypeStr);
			if (GetPieceWithType (pieceType) == null) {
				myTarget.pieces.Add (new PieceData(pieceType));
			}
		}
		scrollPos = EditorGUILayout.BeginScrollView (scrollPos);
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("",GUILayout.MinWidth(120*2+3),GUILayout.MaxWidth(120*2+3));
		EditorGUILayout.LabelField ("Default",GUILayout.MinWidth(60),GUILayout.MaxWidth(60));
		foreach (PieceData piece in ((PieceDatabase)target).pieces) {
			EditorGUILayout.LabelField (piece.type.ToString (),GUILayout.MinWidth(60),GUILayout.MaxWidth(60));
		}
		EditorGUILayout.EndHorizontal ();

		foreach (PieceData piece in ((PieceDatabase)target).pieces) {
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField (piece.type.ToString(),GUILayout.MaxWidth(120));
			piece.prefab = (Piece)EditorGUILayout.ObjectField (piece.prefab,(typeof(Piece)),GUILayout.MinWidth(120),GUILayout.MaxWidth(120));
			if (piece.prefab != null) {
				piece.prefab.Type = piece.type;
				SetPropColor (((int)(piece.prefab.CollisionPropertyDefault)));
				piece.prefab.CollisionPropertyDefault = (CollisionProperty)EditorGUILayout.EnumPopup(piece.prefab.CollisionPropertyDefault,GUILayout.MinWidth(60),GUILayout.MaxWidth(60));
				GUI.color = Color.white;

				foreach(PieceType pieceType in Enum.GetValues(typeof(PieceType))) {
					var collisionPropEntry = piece.GetCollisionPropertyEntry (pieceType);
					if (collisionPropEntry != null) {
						SetPropColor (((int)(collisionPropEntry.collisionProperty)));
						collisionPropEntry.collisionProperty = (CollisionProperty)EditorGUILayout.EnumPopup (collisionPropEntry.collisionProperty, GUILayout.MinWidth (60), GUILayout.MaxWidth (60));
						GUI.color = Color.white;
					} else {
						EditorGUILayout.LabelField ("",GUILayout.MaxWidth(60));
					}
				}

				EditorUtility.SetDirty (piece.prefab);
			}
			EditorGUILayout.EndHorizontal ();
		}

		EditorGUILayout.EndScrollView ();
		GUILayout.Space (50);

		EditorGUILayout.LabelField ("Add/remove collision entry");
		EditorGUILayout.BeginHorizontal ();
		pieceTypeA = (PieceType)EditorGUILayout.EnumPopup (pieceTypeA,GUILayout.Width(100));
		pieceTypeB = (PieceType)EditorGUILayout.EnumPopup (pieceTypeB,GUILayout.Width(100));
		if (GUILayout.Button ("+", GUILayout.Width (20))) {
			AddCollisionEntry (pieceTypeA,pieceTypeB);
		}
		if (GUILayout.Button ("-", GUILayout.Width (20))) {
			RemoveCollisionEntry (pieceTypeA,pieceTypeB);
		}
		EditorGUILayout.EndHorizontal ();

		if (GUI.changed) {
			EditorUtility.SetDirty (myTarget);
		}
	}

	void SetPropColor(int colorId) {
		if (colorId == 1) {
			GUI.color = new Color(0.8f,1f,0.8f,1);
		}
		if (colorId == 2) {
			GUI.color = new Color(1f,1f,0.8f,1);
		}
	}

	void AddCollisionEntry(PieceType a, PieceType b) {
		var piece = GetPieceWithType (a);
		if (!piece.CollisionPropertyList.Exists ((CollisionPropertyEntry c) => {return c.pieceType==b;})) {
			piece.CollisionPropertyList.Add (new CollisionPropertyEntry (b));
		}
	}

	void RemoveCollisionEntry(PieceType a, PieceType b) {
		var piece = GetPieceWithType (a);
		var index = piece.CollisionPropertyList.FindIndex((CollisionPropertyEntry c) => { return c.pieceType == b;});
		if (index > -1) {
			piece.CollisionPropertyList.RemoveAt (index);
		}
	}

	PieceType pieceTypeA;
	PieceType pieceTypeB;

	PieceData GetPieceWithType(PieceType type) {
		foreach(PieceData piece in ((PieceDatabase)target).pieces) {
			if (piece.type == type) {
				return piece;
			}
		}
		return null;
	}
}
#endif