using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : DynamicBody {
	protected override void OnStart() {
		speed = 3;
	}

	protected override void OnUpdate() {
		Check(dir*speed*MovingDir, (Piece[] ps, bool b) => {
			if (ExistPiece(ps, (Piece p) => { return p.Type==PieceType.Block && !((Block)p).IsSticky(dir*speed*MovingDir);})) {
				MovingDir *= -1;
			}
		}
		);
	}

	public override void Init (PieceLevelData pieceLevelData, GameLogic gameLogic) {
		SetGravity (pieceLevelData.dir);
		MovingDir = pieceLevelData.flipX ? 1 : -1;
	}

	public override void Hit (Piece hitPiece, Vector3 direction)
	{
		float tmpThreshold = 0.1f;

		if (hitPiece.Type == PieceType.Hero) {
			var verticalDotProduct = Vector3.Dot (direction, new Vector3(dir.y, -dir.x,dir.z));

			if (verticalDotProduct < -tmpThreshold) {
				hitPiece.GetComponent<Hero> ().SmallJump ();
				Destroy (this.gameObject);
			} else {
				hitPiece.Destroy();
			}
		}
	}
}
