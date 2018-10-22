using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionPiece : Piece {
	PieceLevelData pieceLevelData;

	public override void Init (PieceLevelData pieceLevelData, GameLogic gameLogic) {
		this.pieceLevelData = pieceLevelData;
	}

	bool IsMatchingDirection(Vector3 direction, Direction dir) {
		float tmpThreshold = 0.1f;

		switch (dir) {
		case Direction.Up:
			return (direction.y < -tmpThreshold);
		case Direction.Right:
			return (direction.x < -tmpThreshold);
		case Direction.Down:
			return (direction.y > tmpThreshold);
		case Direction.Left:
			return (direction.x > tmpThreshold);
		}
		return false;
	}

	public override void Hit (Piece hitPiece, Vector3 direction)
	{
		if (hitPiece.Type == PieceType.Enemy1) {
			if (IsMatchingDirection(direction, pieceLevelData.dir) && !isFunctionRunning) {
				StartCoroutine (FunctionCr (hitPiece));
			}
		}
		if (hitPiece.Type == PieceType.Hero) {
			if (pieceLevelData.GetSpecificData<FunctionPieceLevelData> ().type == FunctionPieceLevelData.FunctionType.LeaveWorld && !Director.TransitionManager.isPlayingTransition) {
				Director.TransitionManager.PlayTransition (() => {UnityEngine.SceneManagement.SceneManager.LoadScene ("WorldSelectScene");},0.1f,Director.TransitionManager.FadeToBlack(),Director.TransitionManager.FadeOut());
			}
			var camera = GameObject.Find ("MiddleCamera").GetComponent<Camera> ();
			if (IsMatchingDirection(direction, pieceLevelData.dir) && pieceLevelData.GetSpecificData<FunctionPieceLevelData> ().type == FunctionPieceLevelData.FunctionType.ChangeLayer && !GameObject.Find ("BaseCamera").GetComponentInParent<CameraManager>().IsLayerVisible()/*&& !camera.enabled*/) {
				//LevelInit.SetLayerRecursively (hitPiece.gameObject, LayerMask.NameToLayer ("LevelLayer1"));
				//Debug.LogError ("ChangeLayer Hit");
				//GameObject.Find ("MiddleCamera").GetComponent<Camera> ().enabled = true;
				GameObject.Find ("BaseCamera").GetComponentInParent<CameraManager>().ShowLayer();
			}
			if (IsMatchingDirection(direction, pieceLevelData.dir) && pieceLevelData.GetSpecificData<FunctionPieceLevelData> ().type == FunctionPieceLevelData.FunctionType.ChangeLayerBack && GameObject.Find ("BaseCamera").GetComponentInParent<CameraManager>().IsLayerVisible()/*&& camera.enabled*/) {
				//LevelInit.SetLayerRecursively (hitPiece.gameObject, LayerMask.NameToLayer ("LevelLayer0"));
				//Debug.LogError ("ChangeLayerBack Hit");
				//GameObject.Find ("MiddleCamera").GetComponent<Camera> ().enabled = false;
				GameObject.Find ("BaseCamera").GetComponentInParent<CameraManager>().HideLayer();
			}
		}
	}

	bool isFunctionRunning;
	IEnumerator FunctionCr(Piece hitPiece) {
		isFunctionRunning = true;
		yield return new WaitForSeconds (pieceLevelData.GetSpecificData<FunctionPieceLevelData> ().delay);


		if (hitPiece == null) yield break;
		if (pieceLevelData.GetSpecificData<FunctionPieceLevelData>().type == FunctionPieceLevelData.FunctionType.Turn) {
			hitPiece.GetComponent<Enemy> ().TurnAround ();
		}
		if (pieceLevelData.GetSpecificData<FunctionPieceLevelData>().type == FunctionPieceLevelData.FunctionType.Jump) {
			hitPiece.GetComponent<Enemy> ().SmallJump ();
		}
		
		yield return new WaitForSeconds (pieceLevelData.GetSpecificData<FunctionPieceLevelData> ().cooldown);
		isFunctionRunning = false;
	}
}
