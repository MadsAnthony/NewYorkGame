using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : Piece {

	public GameObject spriteTile;
	public GameObject[] SideGameObjects;
	public GameObject[] SideGameObjectsBone;
	public GameObject[] SideGameObjectsGoo;
	PieceLevelData pieceLevelData;
	BlockPieceLevelData.SideType[] sides = new BlockPieceLevelData.SideType[4];
	public Sprite[] innerBlockSprites;
	public Sprite[] sideSprites;

	public override void Init (PieceLevelData pieceLevelData, GameLogic gameLogic) {
		spriteTile.GetComponent<SpriteRenderer>().sprite = innerBlockSprites[(int)gameLogic.level.graphicsType];
		if (String.IsNullOrEmpty (pieceLevelData.specificDataJson)) return;
		var specific = pieceLevelData.GetSpecificData<BlockPieceLevelData>();

		this.pieceLevelData = pieceLevelData;
		int i = 0;
		foreach (var side in specific.sides) {
			sides[i] = side;
			i++;
		}

		i = 0;
		foreach (var sideGameObject in SideGameObjects) {
			sideGameObject.GetComponent<SpriteRenderer>().sprite = (gameLogic.level.graphicsType == GraphicsType.World1)? sideSprites[0] : sideSprites[1];
			sideGameObject.SetActive (false);
			if (specific.sides [i] == BlockPieceLevelData.SideType.Normal) {
				sideGameObject.SetActive (false);
				//sideGameObject.GetComponent<SpriteRenderer> ().color = new Color (0.2f, 0.2f, 0.2f, 1);
			}
			if (specific.sides [i] == BlockPieceLevelData.SideType.Sticky) {
				sideGameObject.SetActive (true);
				//sideGameObject.GetComponent<SpriteRenderer> ().color = new Color (0.6f, 0.6f, 0.6f, 1);
			}
			if (specific.sides [i] == BlockPieceLevelData.SideType.Colorable) {
				sideGameObject.SetActive (true);
				sideGameObject.GetComponent<SpriteRenderer> ().color = new Color (1f, 0.7f, 1f, 1);
				gameLogic.coloredBlocksGoal++;
			}
			i++;
		}
		i = 0;
		foreach (var sideGameObjectBone in SideGameObjectsBone) {
			sideGameObjectBone.SetActive (false);
			if (specific.sides [i] == BlockPieceLevelData.SideType.Normal) {
				sideGameObjectBone.SetActive (true);
				sideGameObjectBone.GetComponent<SpriteRenderer> ().color = new Color (246f/255, 255f/255, 191f/255, 1);
			}
			i++;
		}
		i = 0;
		foreach (var sideGameObjectGoo in SideGameObjectsGoo) {
			sideGameObjectGoo.SetActive (false);
			if (specific.sides [i] == BlockPieceLevelData.SideType.Colorable) {
				sideGameObjectGoo.SetActive (true);
				//sideGameObjectGoo.GetComponent<SpriteRenderer> ().color = new Color (0.8f, 0.8f, 0.8f, 1);
			}
			i++;
		}

		if (specific.isPassable) {
			CollisionPropertyDefault = CollisionProperty.Passable;
		}
	}

	public bool IsSticky(Vector3 incommingDir) {
		float tmpThreshold = 0.1f;
		bool endResult = false;
		if (incommingDir.x < -tmpThreshold) {
			endResult |= IsSideSticky(sides[(int)Direction.Right]);
		}
		if (incommingDir.x > tmpThreshold) {
			endResult |= IsSideSticky(sides[(int)Direction.Left]);
		}
		if (incommingDir.y > tmpThreshold) {
			endResult |= IsSideSticky(sides[(int)Direction.Down]);
		}
		if (incommingDir.y < -tmpThreshold) {
			endResult |= IsSideSticky(sides[(int)Direction.Up]);
		}
		return endResult;
	}

	public bool IsSideSticky(BlockPieceLevelData.SideType sideType) {
		return (sideType == BlockPieceLevelData.SideType.Sticky || sideType == BlockPieceLevelData.SideType.Colorable);
	}

	Direction GetDirectionFromVector(Vector3 direction) {
		float tmpThreshold = 0.1f;

		if (direction.x < -tmpThreshold) {
			return Direction.Right;
		}
		if (direction.x > tmpThreshold) {
			return Direction.Left;
		}
		if (direction.y > tmpThreshold) {
			return Direction.Down;
		}
		if (direction.y < -tmpThreshold) {
			return Direction.Up;
		}

		return Direction.Up;
	}

	public override void Hit (Piece hitPiece, Vector3 direction)
	{
		if (hitPiece.Type == PieceType.Hero) {
			Direction tmpDir = GetDirectionFromVector (direction);

			if (sides [(int)tmpDir] == BlockPieceLevelData.SideType.Colorable && SideGameObjectsGoo [(int)tmpDir].activeSelf/*SideGameObjects [(int)tmpDir].GetComponent<SpriteRenderer> ().color != Color.green*/) {
				//SideGameObjects [(int)tmpDir].GetComponent<SpriteRenderer> ().color = Color.green;
				SideGameObjectsGoo [(int)tmpDir].SetActive (false);
				Director.GameEventManager.Emit (GameEventType.BlockColored);
			}
		}

		if (hitPiece.Type == PieceType.Enemy1) {
			Direction tmpDir = GetDirectionFromVector (direction);

			if (sides [(int)tmpDir] == BlockPieceLevelData.SideType.Colorable && !SideGameObjectsGoo [(int)tmpDir].activeSelf/*SideGameObjects [(int)tmpDir].GetComponent<SpriteRenderer> ().color == Color.green*/) {
				//SideGameObjects [(int)tmpDir].GetComponent<SpriteRenderer> ().color = Color.white;
				SideGameObjectsGoo [(int)tmpDir].SetActive (true);
				Director.GameEventManager.Emit (GameEventType.BlockUnColored);
			}
		}
	}
}
