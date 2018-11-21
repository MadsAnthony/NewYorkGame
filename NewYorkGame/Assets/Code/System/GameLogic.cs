using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Spine.Unity;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour {
	[SerializeField] private SkeletonAnimation kingKong;
	[SerializeField] private SkeletonAnimation maleCharacter;
	[SerializeField] private MaleCharacterDrop maleCharacterDrop;
	[SerializeField] private GameObject cameraParent;
	[SerializeField] private AnimationCurve moveCameraCurve;
	[SerializeField] private GameObject endTitle;

	[SerializeField] private GameObject livesBar;
	[SerializeField] private Text livesText;

	public int collectablesCollected;
	public int collectablesGoal;
	public int coloredBlocksGoal;

	public int CurrentColoredBlocks {get {return currentColoredBlocks;}}
	int currentColoredBlocks = 0;
	public int CollectablesCollected { get; set;}
	public LevelAsset level;
	public float time = 0;
	bool stopTimer = false;
	bool isRestartingLevel;
	public Hero hero;

	void Start () {
		maleCharacter.gameObject.SetActive (false);
		maleCharacterDrop.gameObject.SetActive (false);
		endTitle.gameObject.SetActive (false);
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
			StartCoroutine (WinLevel());
			break;
		case GameEventType.PieceDestroyed:
			Piece tmpPiece = ((Piece)e.context);
			if (tmpPiece.Type == PieceType.Hero) {
				hero.LooseLife ();
				if (hero.Lives <= 0) {
					StartCoroutine (RestartLevelCr (0.2f));
				}
			}
			break;
		}
	}

	private IEnumerator WinLevel() {
		livesBar.gameObject.SetActive (false);
		hero.spine.AnimationState.SetAnimation (0, "Idle", true);
		kingKong.AnimationState.SetAnimation (0, "Fall", false);
		maleCharacterDrop.Drop ();
		yield return MoveGameObject (cameraParent, cameraParent.transform.localPosition.x + 8, cameraParent.transform.localPosition.y, 1);
		yield return new WaitForSeconds (1.5f);
		maleCharacter.gameObject.SetActive (true);
		yield return MoveGameObject (cameraParent, cameraParent.transform.localPosition.x - 10, cameraParent.transform.localPosition.y, 1);
		var heroSpineLocalScale = hero.spine.transform.localScale;
		hero.spine.transform.localScale = new Vector3 (heroSpineLocalScale.x*-1,heroSpineLocalScale.y,heroSpineLocalScale.z);
		hero.spine.AnimationState.SetAnimation (0, "Run", true);
		yield return MoveGameObject (hero.gameObject, hero.transform.localPosition.x - 3, hero.transform.localPosition.y, 1);
		hero.spine.AnimationState.SetAnimation (0, "Idle", true);
		yield return MoveGameObject (cameraParent, cameraParent.transform.localPosition.x - 3.5f, cameraParent.transform.localPosition.y, 1);
		yield return new WaitForSeconds (1);
		yield return MoveGameObject (cameraParent, cameraParent.transform.localPosition.x, cameraParent.transform.localPosition.y+20, 0.2f);
		endTitle.gameObject.SetActive (true);
	}

	private IEnumerator MoveGameObject(GameObject gameObject, float newX, float newY, float duration) {
		var startPos = gameObject.transform.localPosition;
		float t = 0;
		while (t < 1) {
			t += duration*Time.deltaTime;
			var actualT = moveCameraCurve.Evaluate(t);
			var x = Mathf.Lerp (startPos.x,newX,actualT);
			var y = Mathf.Lerp (startPos.y,newY,actualT);
			gameObject.transform.localPosition = new Vector3 (x, y, gameObject.transform.localPosition.z);
			yield return null;
		}

		gameObject.transform.localPosition = new Vector3 (newX, newY, gameObject.transform.localPosition.z);

		yield break;
	}

	IEnumerator RestartLevelCr(float delay) {
		if (isRestartingLevel) yield break;
		isRestartingLevel = true;
		yield return new WaitForSeconds(delay);
		Director.TransitionManager.PlayTransition (() => {SceneManager.LoadScene ("LevelScene");},0,Director.TransitionManager.FadeToColor(new Color(1,1,1,1),0.2f),Director.TransitionManager.FadeOut(0.2f));
	}

	// Update is called once per frame
	void Update () {
		livesText.text = hero.Lives.ToString();

		if (!stopTimer) {
			time += Time.deltaTime;
		}
	}
}
