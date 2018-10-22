using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour {
	public int collectablesCollected;
	public int collectablesGoal;
	public int coloredBlocksGoal;

	public int CurrentColoredBlocks {get {return currentColoredBlocks;}}
	int currentColoredBlocks = 0;
	public int CollectablesCollected { get; set;}
	public LevelAsset level;
	public float time = 0;
	bool stopTimer = false;

	public Hero hero;
	// Use this for initialization
	void Start () {
		Director.GameEventManager.OnGameEvent += HandleGameEvent;
	}

	void OnDestroy() {
		Director.GameEventManager.OnGameEvent -= HandleGameEvent;
	}

	void HandleGameEvent(GameEvent e) {
		switch (e.type) {
		case GameEventType.BlockColored:
			currentColoredBlocks++;
			if (currentColoredBlocks >= coloredBlocksGoal) {
				Director.GameEventManager.Emit (GameEventType.LevelCompleted);
			}
			break;
		case GameEventType.BlockUnColored:
			currentColoredBlocks--;
			break;
		case GameEventType.CollectableCollected:
			CollectablesCollected++;
			break;
		case GameEventType.LevelCompleted:
			stopTimer = true;
			hero.StopMoving ();
			break;
		case GameEventType.PieceDestroyed:
			Piece tmpPiece = ((Piece)e.context);
			if (tmpPiece.Type == PieceType.Hero) {
				StartCoroutine (RestartLevelCr (0.2f));
			}
			break;
		}
	}

	IEnumerator RestartLevelCr(float delay) {
		yield return new WaitForSeconds(delay);
		Director.TransitionManager.PlayTransition (() => {SceneManager.LoadScene ("LevelScene");},0,Director.TransitionManager.FadeToColor(new Color(1,1,1,1),0.2f),Director.TransitionManager.FadeOut(0.2f));
		//SceneManager.LoadScene ("LevelScene");
	}

	// Update is called once per frame
	void Update () {
		if (!stopTimer) {
			time += Time.deltaTime;
		}
	}
}
