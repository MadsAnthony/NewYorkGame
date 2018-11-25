using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Hero : DynamicBody {
	public GameObject spinePivot;
	public SkeletonAnimation spine;
	public GameObject sprite;
	private Vector3 spriteStartScale;
	public int levelDoorIndex;
	private bool isInvisible;
	public int Lives = 3;

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

		if (!stopMoving && IsOnGround && MovingDir == 0 && spine.AnimationState.GetCurrent(0).ToString() != "Idle" && (spine.AnimationState.GetCurrent(0).Loop || spine.AnimationState.GetCurrent (0).IsComplete)) {
			spine.AnimationState.SetAnimation (0, "Idle", true);
			spine.transform.localEulerAngles += new Vector3 (0,0,50)*Time.deltaTime;
		}

		if (IsOnGround && gravity<=0 && MovingDir!=0 && spine.AnimationState.GetCurrent(0).ToString() != "Run" && spine.AnimationState.GetCurrent(0).ToString() != "Slide") {
			spine.AnimationState.SetAnimation (0, "Run", true);
		}

		var towerWidth = 8f;
		if (Mathf.Abs(transform.localPosition.x)>towerWidth && ChangingDirIndex == 0) {
			var sign = (int)Mathf.Sign(transform.localPosition.x);
			ChangeDir (sign);
			transform.localPosition = new Vector3 (towerWidth*sign, transform.localPosition.y, transform.localPosition.z);
		}
		if (Mathf.Abs(transform.localPosition.z)>towerWidth  && ChangingDirIndex == 1) {
			var sign = (int)Mathf.Sign(transform.localPosition.z);
			ChangeDir (sign);
			transform.localPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y, towerWidth*sign);
		}
		if (Mathf.Abs(transform.localPosition.x)>towerWidth && ChangingDirIndex == 2) {
			var sign = -(int)Mathf.Sign(transform.localPosition.x);
			ChangeDir (sign);
			transform.localPosition = new Vector3 (-towerWidth*sign, transform.localPosition.y, transform.localPosition.z);
		}
		if (Mathf.Abs(transform.localPosition.z)>towerWidth && ChangingDirIndex == 3) {
			var sign = -(int)Mathf.Sign(transform.localPosition.z);
			ChangeDir (sign);
			transform.localPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y, -towerWidth*sign);
		}
		if (transform.localPosition.y>37 && IsOnGround) {
			spine.transform.localEulerAngles = new Vector3 (0,0,0);
		}

	}

	public void LooseLife() {
		if (!isInvisible) {
			Lives -= 1;
			isInvisible = true;
			StartCoroutine (CooldownForInvisible());
		}
	}

	public IEnumerator CooldownForInvisible() {
		yield return new WaitForSeconds (0.5f);
		isInvisible = false;
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

			/*if (Mathf.Abs (horizontalDotProduct) > threshold && !touchConsumed) {
				int newMovingDir = -1 * (int)Mathf.Sign (horizontalDotProduct);
				touchConsumed = newMovingDir != MovingDir;
				MovingDir = newMovingDir;
			}*/

			if (IsOnGround && !touchConsumed && !upMovementConsumed && noGravityT>=1 && gravity<=0) {
				Jump ();
				upMovementConsumed = true;
				EndJump ();

				spine.AnimationState.SetAnimation (0, "Jump", false);

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
