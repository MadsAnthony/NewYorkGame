using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : Piece {
	float gravity;
	void Update () {
		gravity -= 1f;
		gravity = Mathf.Clamp (gravity,-10,10);
		Move (new Vector3 (0, gravity, 0));
	}

	public override void Init (PieceLevelData pieceLevelData, GameLogic gameLogic) {
	}

	public override void Hit (Piece hitPiece, Vector3 direction)
	{
	}

}
