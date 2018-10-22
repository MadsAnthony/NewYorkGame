using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDestructible : Piece {
	private BlockDestructiblePieceLevelData specific;
	public Sprite[] hitsSprites;
	CollisionProperty oldCollisionPropertyDefault;
	int hits;
	int Hits { 
		get { return hits; }
		set {
			hits = value;
			GetComponentInChildren<SpriteRenderer> ().sprite = hitsSprites [Mathf.Clamp(hits-1,0,hitsSprites.Length-1)];
			if (hits <= 0) {
				oldCollisionPropertyDefault = CollisionPropertyDefault;
				CollisionPropertyDefault = CollisionProperty.Passable;
				gameObject.SetActive (false);
			}
		}
	}


	public override void Init (PieceLevelData pieceLevelData, GameLogic gameLogic) {
		specific = pieceLevelData.GetSpecificData<BlockDestructiblePieceLevelData> ();
		Hits = specific.hits;
	}

	public void Respawn() {
		CollisionPropertyDefault = oldCollisionPropertyDefault;
		Hits = specific.hits;
	}

	public override void Hit (Piece hitPiece, Vector3 direction)
	{
		if (hitPiece.Type == PieceType.Boss1) {
			Hits -= 1;
		}
		if (hitPiece.Type == PieceType.Hero && hitPiece.GetComponent<Hero>().Gravity<=-hitPiece.GetComponent<Hero>().maxGravity) {
			Hits -= 1;
		}
	}
}
