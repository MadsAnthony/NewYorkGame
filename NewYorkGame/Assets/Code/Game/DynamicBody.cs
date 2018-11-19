using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DynamicBody : Piece {
	private const float NORMAL_SPEED = 7;
	private const float GRAVITY_NORMAL_ACC = 0.7f;

	protected float gravityAcc = GRAVITY_NORMAL_ACC;
	protected float gravity;
	protected float speed = NORMAL_SPEED;
	public float maxGravity = 50;
	protected Vector3 dir;

	Vector3[] dirs = new Vector3[4]{new Vector3(-1,-0.2f,0),
									new Vector3(0,-0.2f,-1),
									new Vector3(1,-0.2f,0),
									new Vector3(0,-0.2f,1)
									};
	protected int dirsIndex;

	bool isOnGround;
	bool isInWater;

	public float Gravity {get { return gravity;}}

	protected int oldMovingDir;
	private int movingDir = -1;
	public int MovingDir {
		get 
		{ 
			return movingDir;
		}
		protected set
		{
			if (movingDir != 0) oldMovingDir = movingDir;
			movingDir = value;
			if (OnMovingDirChangeValue != null) {
				OnMovingDirChangeValue (movingDir);
			}
		}
	}
	public Action<int> OnMovingDirChangeValue;

	// Use this for initialization
	void Start () {
		dir = dirs[dirsIndex];

		OnStart ();
	}

	protected float noGravityT = 1;
	// Update is called once per frame
	void Update () {
		if (noGravityT < 1) {
			noGravityT += 0.025f;
		}
		noGravityT = Mathf.Clamp (noGravityT, 0, 1);

		gravity -= gravityAcc*noGravityT;

		OnUpdate ();

		Move(dir*speed*MovingDir,(Piece[] ps, bool b) => {
			if (ExistPiece(ps, (Piece p) => {return p.Type==PieceType.Block && ((Block)p).IsSticky(dir*speed*MovingDir);})) {
				ChangeGravity(1*-MovingDir);
			}
		});


		if (Type == PieceType.Hero) {
			isInWater = false;
			var colliders = Physics.OverlapBox(transform.position+GetComponent<BoxCollider>().center, GetComponent<BoxCollider>().bounds.size*0.5f);
			foreach (var col in colliders) {
				if (col.GetComponent<Piece> () != null && col.GetComponent<Piece> ().Type == PieceType.Water) {
					isInWater = true;
				}
			}

			if (isInWater) {
				if (speed > 3) {
					Director.Sounds.waterSplash.Play ();
					gravity *= 0.2f;
					gravityAcc = 0.5f;
					speed = 3;
				}
			} else {
				if (speed<NORMAL_SPEED) {
					Director.Sounds.waterSplash.Play ();
					gravityAcc = GRAVITY_NORMAL_ACC;
					speed = NORMAL_SPEED;
				}
			}
		}

		Vector3 gravityDir = new Vector3 (0,1,0);

		Vector3 tmpMoveDir = gravityDir * Mathf.Clamp (gravity, -maxGravity, maxGravity);

		Move(tmpMoveDir,
			(Piece[] ps, bool b) => {
					if (gravity<=0) {
					// When ground is hit
					if (gravity<=-(maxGravity)) {
						OnSmash();
						Director.Sounds.breakSound.Play ();
						Director.CameraShake();
					}

					gravity = 0;
					IsOnGround = true;

					// If all ground blocks are non sticky, then fall down.
					if (AllPiece(ps, (Piece p) => {return p.Type==PieceType.Block && !((Block)p).IsSticky(tmpMoveDir);}) && dirsIndex%4 != 0) {
						MovingDir = dirsIndex%4 != 2? 0 : MovingDir*-1;
						ChangeGravity(-dirsIndex);
						}
					} else {
					// When ceil is hit

					// If ceiling is block then stop any jump.
					if (ExistPiece(ps, (Piece p) => {return p.Type==PieceType.Block;})) {
						gravity = 0;
						EndJump();
					}
					// If one ceil block are sticky, then stick to that.
					if (ExistPiece(ps, (Piece p) => {return p.Type==PieceType.Block && ((Block)p).IsSticky(tmpMoveDir);})) {
						IsOnGround = true;
						gravity = 0;
						MovingDir *= -1;
						ChangeGravity(2);
					}
				}
				},
			(Piece[] ps2, bool b2) => {
				// if falling, then check if it is possible to stick to nearby wall.
				IsOnGround = false;
				Check(dir*speed*-MovingDir, (Piece[] ps, bool b) => {
					if (gravity<0f) {
						if (ExistPiece(ps, (Piece p) => { return p.Type==PieceType.Block && ((Block)p).IsSticky(dir*speed*-MovingDir);})) {
							ChangeGravity(MovingDir);
							}
						}
					});
				});
	}

	public bool IsOnGround { 
		get { 
			return isOnGround;
		} 
		set {
			if (value == true) {
				CancelInvoke("IsNotOnGround");
				isOnGround = value;
			} else {
				Invoke("IsNotOnGround",0.1f);
			}
		}
	}

	void IsNotOnGround() {
		isOnGround = false;
	}

	protected void Jump(float jumpForce = 14) {
		Director.Sounds.jump.Play ();
		gravity = jumpForce;
		noGravityT = 0;
	}

	protected void EndJump() {
		noGravityT = 1;
	}

	public void SmallJump(float jumpForce = 12) {
		gravity = jumpForce;
	}

	protected bool stopMoving = false;
	public void StopMoving() {
		stopMoving = true;
		MovingDir = 0;
	}
	public void StartMoving(int moveDir = 0) {
		stopMoving = false;
		MovingDir = moveDir;
	}

	public void TurnAround() {
		MovingDir *= -1;
	}

	public void SetGravity(Direction dir) {
		if (dir == Direction.Up) {
			dirsIndex = 2;
		}
		if (dir == Direction.Left) {
			dirsIndex = 3;
		}
		if (dir == Direction.Right) {
			dirsIndex = 1;
		}
		if (dir == Direction.Down) {
			dirsIndex = 0;
		}
	}

	protected virtual void OnStart() {
	}

	protected virtual void OnUpdate() {
	}

	protected virtual void OnSmash() {
	}

	protected virtual void ChangeGravity(int delta) {
		dirsIndex += delta;
		if (dirsIndex < 0) {
			dirsIndex = dirs.Length-1;
		}
		dir = dirs [dirsIndex%dirs.Length];
		gravity = 0;
	}

	public int ChangingDirIndex { get; private set;}

	protected void ChangeDir(int delta) {
		ChangingDirIndex += delta;
		if (ChangingDirIndex < 0) {
			ChangingDirIndex = dirs.Length-1;
		}
		ChangingDirIndex = ChangingDirIndex%dirs.Length;
		dir = dirs [ChangingDirIndex];
	}

}
