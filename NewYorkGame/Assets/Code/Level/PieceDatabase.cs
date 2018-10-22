using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PieceDatabase", menuName = "Level/New PieceDatabase", order = 1)]
public class PieceDatabase : ScriptableObject {
	public List<PieceData> pieces = new List<PieceData>();

	public PieceData GetPieceData(PieceType type) {
		foreach(PieceData pieceData in pieces) {
			if (pieceData.type == type) {
				return pieceData;
			}
		}
		return null;
	}
}

// ATTENTION: Always add new entries at the end and be careful when removing entries (as enums are serialized to integers).
public enum PieceType {
	Enemy1,
	FunctionPiece,
	Block,
	Spike,
	Hero,
	Collectable,
	BlockDestructible,
	Ball,
	Water,
	LevelDoor,
	Boss1
};

[Serializable]
public class PieceData {
	public PieceType type;
	public Piece prefab;
	public List<CollisionPropertyEntry> CollisionPropertyList = new List<CollisionPropertyEntry> ();

	public PieceData(PieceType type) {
		this.type = type;
	}

	public CollisionPropertyEntry GetCollisionPropertyEntry(PieceType b) {
		return CollisionPropertyList.Find ((CollisionPropertyEntry c) => {return c.pieceType == b;});
	}
}

public enum CollisionProperty {Solid, Passable, Pushable};

[Serializable]
public class CollisionPropertyEntry {
	public PieceType pieceType;
	public CollisionProperty collisionProperty;

	public CollisionPropertyEntry(PieceType pieceType) {
		this.pieceType = pieceType;
	}
}
