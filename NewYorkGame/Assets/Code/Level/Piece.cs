using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
public abstract class Piece : MonoBehaviour {
	//[HideInInspector]
	public PieceType Type;
	//[HideInInspector]
	public bool IsPassable(PieceType pieceType) {
		var entry = Director.PieceDatabase.GetPieceData (Type).GetCollisionPropertyEntry (pieceType);
		if (entry != null) {
			return entry.collisionProperty == CollisionProperty.Passable;
		} else {
			return CollisionPropertyDefault == CollisionProperty.Passable;
		}
	}

	public bool IsPushable(PieceType pieceType) {
		var entry = Director.PieceDatabase.GetPieceData (Type).GetCollisionPropertyEntry (pieceType);
		if (entry != null) {
			return entry.collisionProperty == CollisionProperty.Pushable;
		} else {
			return CollisionPropertyDefault == CollisionProperty.Pushable;
		}
	}

	public CollisionProperty CollisionPropertyDefault;

	public abstract void Init (PieceLevelData pieceData, GameLogic gameLogic);

	public abstract void Hit (Piece hitPiece, Vector3 direction);

	public void Destroy() {
		Director.GameEventManager.Emit(new GameEvent(GameEventType.PieceDestroyed, this));
		//gameObject.SetActive(false);
	}

	private float gap = 0.01f;
	private Rigidbody rb;

	void EnsureRigidBody() {
		if (rb == null) {
			if (GetComponent<Rigidbody> () != null) {
				rb = GetComponent<Rigidbody> ();
			} else {
				rb = gameObject.AddComponent<Rigidbody> ();
				rb.isKinematic = true;
			}
		}
	}

	bool FloatApproximatelyEqual(float a, float b, float epsilon = 0.0001f) {
		return (Math.Abs (a - b) < epsilon);
	}

	public Vector3 Move(Vector3 dir, Action<Piece[],bool> callbackInterrupted = null, Action<Piece[],bool> callbackFinished = null, bool useDeltaTime = true, Piece[] excludePieces = null, bool canDestroy = false, bool isJustACheck = false) {
		EnsureRigidBody();

		Vector3 inputDir = dir * (useDeltaTime? Time.deltaTime : 1);
		Vector3 newDir = inputDir;
		List<Piece> interruptingPieces = new List<Piece>();
		List<Piece> hitPieces = new List<Piece>();

		int i = 0;
		Vector3 tmpDir;
		var hits = rb.SweepTestAll (inputDir, inputDir.magnitude);
		List<RaycastHit> sortedHits = hits.ToList ();
		sortedHits.Sort ((x, y) => { 
			if(x.distance.Equals(y.distance)) {
				return 0;
			} else if (x.distance < y.distance){
				return -1;
			} else {
				return 1;
			};
		});
		foreach(var hit in sortedHits) {
			var piece = hit.collider.GetComponent<Piece> ();
			if (piece == null) continue;

			// ignore pieces that are part of excludePieces.
			if (excludePieces != null && excludePieces.Contains(piece)) continue;


			tmpDir = inputDir.normalized * (hit.distance - gap);
			// Do not hit pieces that are farther away (but do hit pieces before).
			if (i > 0 && interruptingPieces.Count>0 && !FloatApproximatelyEqual(newDir.magnitude,tmpDir.magnitude) && tmpDir.magnitude > newDir.magnitude) continue;

			if (!isJustACheck) {
				piece.Hit (this, dir);
			}
			hitPieces.Add(piece);

			if (!piece.IsPassable(this.Type) && !piece.IsPushable(this.Type)) {
				newDir = tmpDir;

				interruptingPieces.Add (piece);
			}
			if (piece.IsPushable(this.Type)) {
				bool shouldDestroy = false;
				newDir = tmpDir+piece.Move((inputDir-tmpDir),(Piece[] ps, bool wasPushing) => {if (!wasPushing) shouldDestroy = true;},null,false, null, false, isJustACheck);
				if (shouldDestroy && canDestroy) {
					newDir = inputDir;
					piece.Destroy();
					continue;
				}

				interruptingPieces.Add (piece);
			}
			i++;
		}
		if (interruptingPieces.Count>0) {
			if (callbackInterrupted != null) {
				callbackInterrupted (interruptingPieces.ToArray (), false);
			}
		}

		if (!isJustACheck) {
			this.transform.position += newDir;
		}

		if (interruptingPieces.Count == 0) {
			if (callbackFinished != null) {
				callbackFinished (hitPieces.ToArray (), false);
			}
		}
		return newDir;
	}

	protected Vector3 Check(Vector3 dir, Action<Piece[],bool> callbackInterrupted = null, Action<Piece[],bool> callbackFinished = null, bool useDeltaTime = true, Piece[] excludePieces = null, bool canDestroy = false, bool isJustACheck = false) {
		return Move (dir,callbackInterrupted,callbackFinished,useDeltaTime,excludePieces,canDestroy,true);
	}

	public bool ExistPiece(Piece[] pieces, Predicate<Piece> condition) {
		foreach (Piece piece in pieces) {
			if (condition(piece)) return true;
		}
		return false;
	}

	public bool AllPiece(Piece[] pieces, Predicate<Piece> condition) {
		foreach (Piece piece in pieces) {
			if (!condition(piece)) return false;
		}
		return true;
	}
}
