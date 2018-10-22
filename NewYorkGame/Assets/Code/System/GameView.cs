using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameView : UIView {
	public GameLogic gameLogic;
	public Text goalText;
	public Text collectableText;
	public Text timer;
	public Text timerBest;

	public Camera camera;
	public GameObject cameraPivot;

	LevelSaveData prevLevelProgress;

	protected override void OnStart () {
		prevLevelProgress = Director.SaveData.GetLevelSaveDataEntry (Director.Instance.LevelIndex.ToString ());
		if (prevLevelProgress != null) {
			TimeSpan timeSpan = TimeSpan.FromSeconds (prevLevelProgress.time);
			timerBest.text = string.Format ("{0:D2}:{1:D2}:{2:D2}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
		} else {
			timerBest.text = String.Empty;
		}

		Director.GameEventManager.OnGameEvent += HandleGameEvent;
	}

	protected override void OnRemoving() {
		Director.GameEventManager.OnGameEvent -= HandleGameEvent;
	}

	void HandleGameEvent(GameEvent e) {
		switch (e.type) {
		case GameEventType.LevelCompleted:
			var tempDict = Director.SaveData.LevelProgress;
			float bestTime = 0;
			if (prevLevelProgress != null) {
				bestTime = Math.Min (prevLevelProgress.time, gameLogic.time);
			} else {
				bestTime = gameLogic.time;
			}
			tempDict[Director.Instance.LevelIndex.ToString()] = new LevelSaveData(true,gameLogic.CollectablesCollected,bestTime);
			Director.SaveData.LevelProgress = tempDict;

			if (Director.Instance.LevelIndex == 5 || Director.Instance.LevelIndex >= Director.LevelDatabase.levels.Count-1) {
				Director.Instance.LevelIndex = 0;
				SceneManager.LoadScene ("LevelScene");
			} else {
				Director.Instance.LevelIndex += 1;
				Director.TransitionManager.PlayTransition (() => {SceneManager.LoadScene ("LevelScene");},0.1f,Director.TransitionManager.FadeToBlack(),Director.TransitionManager.FadeOut());
			}
			break;
		}
	}

	// Update is called once per frame
	void Update () {
		goalText.text = gameLogic.CurrentColoredBlocks+"/"+gameLogic.coloredBlocksGoal;
		collectableText.text = gameLogic.CollectablesCollected+"/"+gameLogic.collectablesGoal;

		TimeSpan timeSpan = TimeSpan.FromSeconds(gameLogic.time);
		timer.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);

		Vector3 distance = Vector3.zero;
		if (gameLogic.hero != null) {
			distance = gameLogic.hero.transform.position - cameraPivot.transform.position;
			cameraPivot.transform.position += Time.deltaTime * 3 * new Vector3 (distance.x, distance.y, 0);
		}

		Vector4 bounds = GetAllCameraBounds ();
		cameraPivot.transform.position = new Vector3(Mathf.Clamp(cameraPivot.transform.position.x,bounds[0],bounds[1]),-Mathf.Clamp(-cameraPivot.transform.position.y,bounds[2],bounds[3]),cameraPivot.transform.position.z);
	}

	bool firstUpdate = true;
	float minX;
	float maxX;
	float minY;
	float maxY;
	Vector4 GetAllCameraBounds() {
		float initialMinX = 0;
		float initialMaxX = gameLogic.level.levelSize.x-20;
		float initialMinY = 0;
		float initialMaxY = gameLogic.level.levelSize.y-30;

		var closestCameraBoundLeft 	= GetClosestCameraBound (Direction.Left);
		var closestCameraBoundRight = GetClosestCameraBound (Direction.Right);
		var closestCameraBoundUp 	= GetClosestCameraBound (Direction.Up);
		var closestCameraBoundDown 	= GetClosestCameraBound (Direction.Down);


		float newMinX = (closestCameraBoundLeft  != null)? 	closestCameraBoundLeft.pos - 2 		 	: 0;
		float newMaxX = (closestCameraBoundRight != null)? 	closestCameraBoundRight.pos + 2 - 20 	: 0;
		float newMinY = (closestCameraBoundUp 	 != null)? 	closestCameraBoundUp.pos - 2 			: 0;
		float newMaxY = (closestCameraBoundDown	 != null)? 	closestCameraBoundDown.pos + 2 - 30		: 0;

		if (firstUpdate) {
			if (closestCameraBoundLeft != null) {
				minX = newMinX;
			}
			if (closestCameraBoundRight != null) {
				maxX = newMaxX;
			}
			if (closestCameraBoundUp != null) {
				minY = newMinY;
			}
			if (closestCameraBoundDown != null) {
				maxY = newMaxY;
			}
			firstUpdate = false;
		}

		if (closestCameraBoundLeft==null) {
			minX = initialMinX;
		} else {
			minX += Time.deltaTime * 3 *(newMinX-minX);
		}
		if (closestCameraBoundRight==null) {
			maxX = initialMaxX;
		} else {
			maxX += Time.deltaTime * 3 *(newMaxX-maxX);
		}
		if (closestCameraBoundUp==null) {
			minY = initialMinY;
		} else {
			minY += Time.deltaTime * 3 *(newMinY-minY);
		}
		if (closestCameraBoundDown==null) {
			maxY = initialMaxY;
		} else {
			maxY += Time.deltaTime * 3 *(newMaxY-maxY);
		}

		// Clamp min max values so min values are not higher than max or max is lower than min.
		minX = Mathf.Min (minX, maxX);
		maxX = Mathf.Max (maxX, minX);
		minY = Mathf.Min (minY, maxY);
		maxY = Mathf.Max (maxY, minY);

		return new Vector4 (minX, maxX, minY, maxY);
	}

	CameraBound GetClosestCameraBound(Direction dir) {
		CameraBound resultCameraBound = null;
		foreach (CameraBound cameraBound in gameLogic.level.cameraBounds) {
			if (cameraBound.dir != dir) continue;
			if (dir == Direction.Left) {
				if (cameraBound.pos + LevelInit.LevelStartPos.x < gameLogic.hero.transform.position.x) {
					if (resultCameraBound == null || cameraBound.pos > resultCameraBound.pos) {
						resultCameraBound = cameraBound;
					}
				}
			}
			if (dir == Direction.Right) {
				if (cameraBound.pos + LevelInit.LevelStartPos.x > gameLogic.hero.transform.position.x) {
					if (resultCameraBound == null || cameraBound.pos < resultCameraBound.pos) {
						resultCameraBound = cameraBound;
					}
				}
			}
			if (dir == Direction.Up) {
				if (-cameraBound.pos + LevelInit.LevelStartPos.y > gameLogic.hero.transform.position.y) {
					if (resultCameraBound == null || cameraBound.pos < resultCameraBound.pos) {
						resultCameraBound = cameraBound;
					}
				}
			}
			if (dir == Direction.Down) {
				if (-cameraBound.pos + LevelInit.LevelStartPos.y < gameLogic.hero.transform.position.y) {
					if (resultCameraBound == null || cameraBound.pos > resultCameraBound.pos) {
						resultCameraBound = cameraBound;
					}
				}
			}
		}
		return resultCameraBound;
	}
}
