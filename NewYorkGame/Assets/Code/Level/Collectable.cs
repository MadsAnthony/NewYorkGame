using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : Piece {

	public override void Init (PieceLevelData pieceLevelData, GameLogic gameLogic) {
	}

	public override void Hit (Piece hitPiece, Vector3 direction)
	{
		if (hitPiece.Type == PieceType.Hero) {
			Director.GameEventManager.Emit (GameEventType.CollectableCollected);
			Destroy(this.gameObject);
		}
	}
}
