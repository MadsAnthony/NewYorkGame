using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : Piece {

	public override void Init (PieceLevelData pieceLevelData, GameLogic gameLogic) {
	}

	public override void Hit (Piece hitPiece, Vector3 direction)
	{
		if (hitPiece.Type == PieceType.Boss1) {
			((Boss1)hitPiece).WasHit();
		}
		if (hitPiece.Type == PieceType.Hero) {
			hitPiece.Destroy();
		}
	}

}
