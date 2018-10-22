using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1 : Piece {
	Vector3 dir = new Vector3(1,0,0);
	float speed = 5;
	int MovingDir = 1;
	int life = 3;

	float[] speedForPhase = new float[]{5,8,12};

	int Life { 
		get { 
			return life;
		} 
		set {
			life = value;
			if (life <= 0) {
				Director.GameEventManager.Emit (GameEventType.LevelCompleted);
			}
			var phase = Mathf.Clamp (3 - life, 0, 2);
			speed = speedForPhase[phase];
		}
	}

	void Start() {
		StartCoroutine ("BossRoutine");
	}

	void Update () {
	}

	IEnumerator BossRoutine() {
		while (true) {
			yield return Patrol ();
			yield return new WaitForSeconds (1f);
			yield return Smash ();
		}
	}

	IEnumerator Patrol() {
		float s = 0;
		bool heroIsTargeted = false;
		while (s<5 && !heroIsTargeted) {
			s += 1 * Time.deltaTime;
			Move(dir*speed*MovingDir,(Piece[] ps, bool b) => {
				if (ExistPiece(ps, (Piece p) => { return p.Type==PieceType.Block;})) {
					MovingDir *= -1;
				}
			});
			var checkDir = new Vector3(0,-1,0);
			if (s>2) {
				Check(checkDir*1000, (Piece[] ps, bool b) => {
					if (ExistPiece(ps, (Piece p) => { return p.Type==PieceType.Hero;})) {
						heroIsTargeted = true;
					}
				}
				);
			}
			yield return null;
		}
	}

	IEnumerator Smash() {
		Vector3 startPos = transform.position;
		bool doneWithSmash = false;
		var smashDir = new Vector3(0,-1,0);
		float smashSpeed = 20;
		while(!doneWithSmash) {
			smashSpeed += 100 * Time.deltaTime;
			Move(smashDir*smashSpeed,(Piece[] ps, bool b) => {
				if (ExistPiece(ps, (Piece p) => { return p.Type==PieceType.Block || p.Type==PieceType.BlockDestructible;})) {
					doneWithSmash = true;
					Director.CameraShake();
				}
			},null,true,null,true);
			yield return null;
		}
		yield return new WaitForSeconds (0.3f);
		smashDir = new Vector3(0,1,0);
		smashSpeed = 8;
		while (transform.position.y<startPos.y) {
			Move(smashDir*smashSpeed,(Piece[] ps, bool b) => {
			});
			yield return null;
		}
	}

	bool isRecovering = false;
	IEnumerator Recover() {
		yield return new WaitForSeconds (0.3f);
		var moveDir = new Vector3(0,1,0);
		var speed = 8;
		while (isRecovering) {
			Move(moveDir*speed,(Piece[] ps, bool b) => {
				if (ExistPiece(ps, (Piece p) => { return p.Type==PieceType.Block;})) {
					isRecovering = false;
				}
			});
			yield return null;
		}

		var levelInit = GameObject.Find ("LevelInit").GetComponent<LevelInit> ();
		foreach (var blockDestructible in levelInit.PiecesByType [PieceType.BlockDestructible]) {
			blockDestructible.gameObject.SetActive (true);
			((BlockDestructible)blockDestructible).Respawn ();
		}

		StartCoroutine ("BossRoutine");
	}

	public void WasHit() {
		if (!isRecovering) {
			Life -= 1;
			isRecovering = true;
			StopCoroutine ("BossRoutine");
			StartCoroutine (Recover ());
		}
	}

	public override void Init (PieceLevelData pieceLevelData, GameLogic gameLogic) {
	}

	public override void Hit (Piece hitPiece, Vector3 direction)
	{
	}
}
