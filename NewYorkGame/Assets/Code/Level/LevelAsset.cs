using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Level", menuName = "Level/New Level", order = 1)]
public class LevelAsset : ScriptableObject {
	public Vector2 levelSize = new Vector2(20,30);
	public List<PieceLevelData> pieces = new List<PieceLevelData>();
	public List<PieceGroupData> pieceGroups = new List<PieceGroupData>();
	public List<BackgroundLevelData> backgroundList = new List<BackgroundLevelData>();
	public List<CameraBound> cameraBounds = new List<CameraBound>();
	public List<LevelLayer> layers = new List<LevelLayer>();
	public GraphicsType graphicsType = GraphicsType.World1;

	public string levelName;

	static public bool HasSpecificLevelData(PieceType pieceType) {
		return (pieceType == PieceType.Block || pieceType == PieceType.FunctionPiece || pieceType == PieceType.LevelDoor || pieceType == PieceType.BlockDestructible);
	}

	public LevelLayer GetLayer(string layerId) {
		foreach(LevelLayer layer in layers) {
			if (layer.id == layerId) {
				return layer;
			}
		}
		return null;
	}
}

public enum BlockType {Normal, Color, Spike, NonSticky, Collectable};
public enum Direction {Up, Right, Down, Left};
public enum BackgroundType {Normal};
public enum GraphicsType {World1,World2,World3};

[Serializable]
public class PieceLevelData {
	public string id;
	public PieceType type;
	public Vector2 pos;
	public Direction dir;
	public bool flipX;
	public string specificDataJson;
	public string layerId;

	public PieceLevelData(PieceType type, Vector2 pos, Direction dir, string layerId) {
		id = Guid.NewGuid ().ToString ();
		this.type = type;
		this.pos  = pos;
		this.dir  = dir;
		this.layerId = layerId;

		if (type == PieceType.Block) {
			SaveSpecificData (new BlockPieceLevelData ());
		}
		if (type == PieceType.FunctionPiece) {
			SaveSpecificData (new FunctionPieceLevelData ());
		}
		if (type == PieceType.LevelDoor) {
			SaveSpecificData (new LevelDoorPieceLevelData ());
		}
		if (type == PieceType.BlockDestructible) {
			SaveSpecificData (new BlockDestructiblePieceLevelData ());
		}
	}

	public T GetSpecificData<T>() {
		return JsonUtility.FromJson<T> (specificDataJson);
	}

	public void SaveSpecificData(object obj) {
		specificDataJson = JsonUtility.ToJson(obj);
	}
}

[Serializable]
public class LevelLayer {
	public string id = Guid.NewGuid ().ToString ();
	public string name;

	public LevelLayer(string name) {
		id = Guid.NewGuid ().ToString ();
		this.name = name;
	}
}

[Serializable]
public class PieceGroupData {
	public string id = Guid.NewGuid ().ToString ();
	public List<string> pieceIds = new List<string>();
	public List<GroupMovement> moves = new List<GroupMovement>();
}

[Serializable]
public class GroupMovement {
	public string id = Guid.NewGuid ().ToString ();
	public Vector2 startPoint;
	public Vector2 endPoint;
	public float delay = 0;
	public float time = 1;
	public AnimationCurve animationCurve = new AnimationCurve();
	public float maxT = 1;
}

[Serializable]
public class BackgroundLevelData {
	public BackgroundType backgroundType;
	public Vector2 pos;
	public string layerId;

	public BackgroundLevelData(BackgroundType backgroundType, Vector2 pos, string layerId) {
		this.backgroundType = backgroundType;
		this.pos = pos;
		this.layerId = layerId;
	}
}

[Serializable]
public class CameraBound {
	public int pos;
	public Direction dir;

	public CameraBound(int pos, Direction dir) {
		this.pos = pos;
		this.dir = dir;
	}
}

[Serializable]
public abstract class SpecificPieceLevelData {
}

[Serializable]
public class BlockPieceLevelData:SpecificPieceLevelData {
	public SideType[] sides   = new SideType[4];
	public SideType[] corners = new SideType[4];

	public bool isPassable;

	public enum SideType {None, Normal, Sticky, Colorable};
}

[Serializable]
public class FunctionPieceLevelData:SpecificPieceLevelData {
	public float delay;
	public float cooldown;
	public FunctionType type;

	public enum FunctionType {Turn, Jump, LeaveWorld, ChangeLayer, ChangeLayerBack};
}

[Serializable]
public class LevelDoorPieceLevelData:SpecificPieceLevelData {
	public int levelIndex;
}

[Serializable]
public class BlockDestructiblePieceLevelData:SpecificPieceLevelData {
	public int hits = 1;
}
