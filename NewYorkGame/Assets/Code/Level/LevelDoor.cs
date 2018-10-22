using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDoor : Piece {
	private PieceLevelData pieceLevelData;
	private GameLogic gameLogic;

	public override void Init (PieceLevelData pieceLevelData, GameLogic gameLogic) {
		this.pieceLevelData = pieceLevelData;
		this.gameLogic = gameLogic;

		var levelIndex = pieceLevelData.GetSpecificData<LevelDoorPieceLevelData> ().levelIndex;
		if (levelIndex > 1 && Director.SaveData.GetLevelSaveDataEntry ((levelIndex - 1).ToString ()) != null) {
			GetComponentInChildren<SpriteRenderer> ().color = new Color (1, 1, 0, GetComponentInChildren<SpriteRenderer> ().color.a);
		} else {
			GetComponentInChildren<SpriteRenderer> ().color = new Color (1, 0, 0, GetComponentInChildren<SpriteRenderer> ().color.a);
		}
		if (Director.SaveData.GetLevelSaveDataEntry (levelIndex.ToString ()) != null) {
			GetComponentInChildren<SpriteRenderer> ().color = new Color(0,1,0,GetComponentInChildren<SpriteRenderer> ().color.a);
		}
	}

	public override void Hit (Piece hitPiece, Vector3 direction)
	{
		if (hitPiece.Type == PieceType.Hero) {
			((Hero)hitPiece).OnALevelDoor ();
			var levelDoorText = Instantiate(Resources.Load("LevelDoorText")) as GameObject;
			levelDoorText.transform.position = new Vector3(transform.position.x,transform.position.y+2,transform.position.z);
			int levelIndex = pieceLevelData.GetSpecificData<LevelDoorPieceLevelData> ().levelIndex;
			((Hero)hitPiece).levelDoorIndex = levelIndex;
			levelDoorText.GetComponentInChildren<TextMesh> ().text = "- level "+ (levelIndex)+" -";

			gameLogic.hero.OnIsOnLevelDoorChangeValue += (bool isOnALevelDoor)=>{if (!isOnALevelDoor) Destroy(levelDoorText);};
		}
	}
}
