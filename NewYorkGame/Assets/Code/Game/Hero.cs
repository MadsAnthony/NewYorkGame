using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : DynamicBody {
	//public SkeletonAnimation spine;
	public GameObject sprite;
	private Vector3 spriteStartScale;
	public int levelDoorIndex;

	protected override void OnStart() {
		spriteStartScale = sprite.transform.localScale;

		OnMovingDirChangeValue += (int movingDir) => { if (IsOnLevelDoor) IsOnLevelDoor = false;};
	}

	protected override void OnUpdate() {
		if (!stopMoving) {
			TouchInput ();
			KeyboardInput ();
		}
		if (MovingDir < 0 || MovingDir > 0) {
			sprite.transform.localScale = new Vector3 (spriteStartScale.x * -MovingDir, spriteStartScale.y, spriteStartScale.z);
		}
		sprite.transform.eulerAngles = new Vector3 (0,0,90*(dirsIndex));

		/*if (IsOnGround && MovingDir == 0 && spine.AnimationState.GetCurrent(0).ToString() != "idle" && (spine.AnimationState.GetCurrent(0).Loop || spine.AnimationState.GetCurrent (0).IsComplete)) {
			spine.AnimationState.SetAnimation (0, "idle", true);
		}

		if (IsOnGround && gravity<=0 && MovingDir!=0 && spine.AnimationState.GetCurrent(0).ToString() != "roll" && (spine.AnimationState.GetCurrent(0).Loop || spine.AnimationState.GetCurrent (0).IsComplete)) {
			spine.AnimationState.SetAnimation (0, "roll", true);
		}*/

		if (Mathf.Abs(transform.localPosition.x)>6 && ChangingDirIndex == 0) {
			var sign = (int)Mathf.Sign(transform.localPosition.x);
			ChangeDir (sign);
			transform.localPosition = new Vector3 (6*sign, transform.localPosition.y, transform.localPosition.z);
		}
		if (Mathf.Abs(transform.localPosition.z)>6  && ChangingDirIndex == 1) {
			var sign = (int)Mathf.Sign(transform.localPosition.z);
			ChangeDir (sign);
			transform.localPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y, 6*sign);
		}
		if (Mathf.Abs(transform.localPosition.x)>6 && ChangingDirIndex == 2) {
			var sign = -(int)Mathf.Sign(transform.localPosition.x);
			ChangeDir (sign);
			transform.localPosition = new Vector3 (-6*sign, transform.localPosition.y, transform.localPosition.z);
		}
		if (Mathf.Abs(transform.localPosition.z)>6 && ChangingDirIndex == 3) {
			var sign = -(int)Mathf.Sign(transform.localPosition.z);
			ChangeDir (sign);
			transform.localPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y, -6*sign);
		}
	}

	protected override void OnSmash() {
		/*spine.AnimationState.SetAnimation (0, "smash", false);
		spine.AnimationState.AddAnimation (0, "idle", true,0.1f);*/
	}

	public void OnALevelDoor() {
		MovingDir = 0;
		IsOnLevelDoor = true;
	}

	private bool isOnLevelDoor;
	public bool IsOnLevelDoor {
		get 
		{ 
			return isOnLevelDoor;
		}
		set 
		{ 
			isOnLevelDoor = value;
			if (OnIsOnLevelDoorChangeValue != null) {
				OnIsOnLevelDoorChangeValue (isOnLevelDoor);
			}
		}
	}
	public Action<bool> OnIsOnLevelDoorChangeValue;


	void KeyboardInput() {
		if (Input.GetKeyDown (KeyCode.UpArrow) && IsOnGround && gravity<=0) {
			Jump ();
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			gravity = -maxGravity;
		}
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			Jump ();
		}
		if (Input.GetKeyUp (KeyCode.UpArrow)) {
			EndJump ();
		}

		if (Input.GetKey (KeyCode.RightArrow)) {
			MovingDir = -1;
		}
		if (Input.GetKey (KeyCode.LeftArrow)) {
			MovingDir = 1;
		}
	}

	Vector3 mouseClickPos;
	float threshold = Mathf.Cos(Mathf.PI*0.25f);
	bool touchConsumed = true;
	bool upMovementConsumed = false;

	Vector3 touchDir = new Vector3(-1,0,0);
	void TouchInput() {
		if (Input.GetMouseButtonDown(0) && touchConsumed) {
			touchConsumed = false;
			mouseClickPos = Input.mousePosition;
		}
		if (Input.GetMouseButtonUp (0)) {
			EndJump ();
			touchConsumed = true;
			upMovementConsumed = false;
		}
		if (Input.GetMouseButton(0)) {
			Vector3 mouseDir = (mouseClickPos - Input.mousePosition);
			var horizontalDotProduct = Vector3.Dot (mouseDir.normalized, touchDir);
			var verticalDotProduct = Vector3.Dot (mouseDir.normalized, new Vector3(touchDir.y, -touchDir.x,touchDir.z));
			var verticalDotProductForce = Vector3.Dot (mouseDir, new Vector3(touchDir.y, -touchDir.x,touchDir.z));

			if (Mathf.Abs (horizontalDotProduct) > threshold && !touchConsumed) {
				int newMovingDir = -1 * (int)Mathf.Sign (horizontalDotProduct);
				touchConsumed = newMovingDir != MovingDir;
				MovingDir = newMovingDir;
			}

			if (verticalDotProduct < -threshold  && IsOnGround && !touchConsumed && !upMovementConsumed && noGravityT>=1 && gravity<=0) {
				Jump ();
				upMovementConsumed = true;

				//spine.AnimationState.SetAnimation (0, "jump", false);

				if (IsOnLevelDoor) {
					MovingDir = oldMovingDir;
				}
			}

			if (verticalDotProduct > threshold && !IsOnGround && !touchConsumed) {
				gravity = -maxGravity;
				touchConsumed = true;
			}
		}
	}

	protected override void ChangeGravity(int delta) {
		base.ChangeGravity(delta);

		// consume all touch inputs - because controls have been changed/rotated.
		touchConsumed = true;
	}

	public override void Init (PieceLevelData pieceLevelData, GameLogic gameLogic) {
		SetGravity (pieceLevelData.dir);
		MovingDir = pieceLevelData.flipX ? 1 : -1;;
	}

	public override void Hit (Piece hitPiece, Vector3 direction) {
		if (hitPiece.Type == PieceType.Enemy1) {
			Destroy ();
		}
	}


}
