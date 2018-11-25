using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroView : MonoBehaviour {

	[SerializeField] private Camera camera;
	[SerializeField] private SpriteRenderer title;
	[SerializeField] private SpriteRenderer pressToStart;
	[SerializeField] private GameObject textBubble;
	[SerializeField] private Text nameText;
	[SerializeField] private Text dialogText;
	[SerializeField] private GameObject maleHead;
	[SerializeField] private GameObject femaleHead;

	[SerializeField] private SkeletonAnimation kingKong;
	[SerializeField] private SkeletonAnimation maleCharacter;

	[SerializeField] private AnimationCurve moveCameraCurve;

	[SerializeField] private GameObject MirrorQuad;

	private bool loadingScene;
	private bool mousePress;
	private bool kingKongAnimationIsDone;
	private void Start() {
		var director = Director.Instance;

		textBubble.SetActive(false);
		kingKong.gameObject.SetActive (false);
		kingKong.AnimationState.TimeScale = 0;
		kingKong.AnimationState.Event += HandleSpineEvent;

		StartCoroutine (IntroFlow());
	}

	private IEnumerator IntroFlow() {
		
		yield return WaitForMouseDown ();
		pressToStart.gameObject.SetActive (false);
		yield return new WaitForSeconds (0.5f);
		yield return FadeTitleDown (2f);
		yield return MoveCamera (0.5f);
		yield return new WaitForSeconds (0.6f);
		yield return ShowDialog ("Woohoo, så er vi her sgu!++ |New York city!", "Maria");
		yield return WaitForMouseDown ();
		yield return ShowDialog ("Ja, det er ret vildt!", "Mads");
		yield return WaitForMouseDown ();
		HideDialog ();
		yield return new WaitForSeconds (0.5f);
		yield return ShowDialog ("Hva?+ Skal vi ik tage hen og se Empire State building?", "Mads");
		yield return WaitForMouseDown ();
		yield return ShowDialog ("Helt sikkert! Lad os gøre det.", "Maria");
		yield return WaitForMouseDown ();
		yield return ShowDialog ("Hvad med en selfie først?", "Maria");
		yield return WaitForMouseDown ();
		yield return new WaitForSeconds (0.5f);
		yield return ShowDialog ("Hmm... okay så.", "Mads");
		yield return WaitForMouseDown ();
		HideDialog ();
		yield return new WaitForSeconds (0.5f);
		yield return ShowDialog ("Wtf?!+ Hvad er det der?", "Mads");
		yield return WaitForMouseDown ();
		yield return ShowDialog ("Shit! Den er jo enorm!", "Maria");
		yield return WaitForMouseDown ();
		yield return ShowDialog ("Er+ det+ en+++ abe?", "Mads");
		yield return WaitForMouseDown ();
		yield return ShowDialog ("Fuck!+ Ja tror du har ret, det er en kæmpe abe!", "Maria");
		yield return WaitForMouseDown ();
		yield return ShowDialog ("Åh nej, den har fået øje på os!", "Maria");
		yield return WaitForMouseDown ();
		HideDialog ();
		yield return ZoomOut (5);
		kingKong.AnimationState.TimeScale = 1;
		kingKong.AnimationState.SetAnimation (0, "Jump", false);
		kingKong.gameObject.SetActive (true);
		yield return new WaitForSeconds (1.5f);
		kingKong.AnimationState.SetAnimation (0, "Grab", false);
		while (!kingKongAnimationIsDone) {
			yield return true;
		}
		yield return new WaitForSeconds (0.5f);
		yield return ZoomIn (5);

		yield return WaitForMouseDown ();
		Director.TransitionManager.PlayTransition (() => { SceneManager.LoadSceneAsync ("LevelScene"); }, 0.1f, Director.TransitionManager.FadeToBlack (), Director.TransitionManager.FadeOut ());
		loadingScene = true;
	}

	private IEnumerator WaitForMouseDown() {
		mousePress = false;
		while (!mousePress) {
			yield return true;
		}
	}

	private IEnumerator FadeTitleDown(float duration) {
		float alpha = 1;
		float t = 0;
		while (t < 1) {
			t += duration*Time.deltaTime;
			alpha = Mathf.Lerp (1,0,t);
			title.color = new Color (title.color.r, title.color.g, title.color.b, alpha);
			yield return null;
		}
		title.gameObject.SetActive (false);
	}

	private void HideDialog() {
		textBubble.SetActive(false);
	}

	private IEnumerator ShowDialog(string dialog, string name) {
		mousePress = false;

		textBubble.SetActive(true);
		maleHead.SetActive (name == "Mads");
		femaleHead.SetActive (name == "Maria");
		string dynamicDialog = "";
		nameText.text = name+":";
		dialogText.text = "";
		yield return new WaitForSeconds (0.1f);
		foreach (var letter in dialog) {
			if (mousePress) {
				dynamicDialog = dialog;
				dynamicDialog = dynamicDialog.Replace ("|", "\n");
				dynamicDialog = dynamicDialog.Replace ("+", "");
				dialogText.text = dynamicDialog;
				yield break;
			}

			if (letter == '|') {
				dynamicDialog += "\n";
				continue;
			}
			if (letter == '+') {
				yield return new WaitForSeconds (0.25f);
				continue;
			}

			dynamicDialog += letter;
			dialogText.text = dynamicDialog;
			if (letter == ',') {
				yield return new WaitForSeconds (0.2f);
			}
			if (letter == '.') {
				yield return new WaitForSeconds (0.4f);
			}
			yield return new WaitForSeconds (0.02f);
		}
	}

	private IEnumerator MoveCamera(float duration) {
		var startPos = camera.transform.localPosition;
		float t = 0;
		while (t < 1) {
			t += duration*Time.deltaTime;
			var actualT = moveCameraCurve.Evaluate(t);
			var y = Mathf.Lerp (startPos.y,1,actualT);
			camera.transform.position = new Vector3 (camera.transform.position.x, y, camera.transform.position.z);
			yield return null;
		}

		camera.transform.position = new Vector3 (camera.transform.position.x, 1, camera.transform.position.z);

		yield break;
	}

	private IEnumerator ZoomOut(float duration) {
		var startPos = camera.transform.localPosition;
		var startSize = camera.orthographicSize;
		float t = 0;
		while (t < 1) {
			t += duration*Time.deltaTime;
			camera.orthographicSize = Mathf.Lerp (startSize,11,t);
			var x = Mathf.Lerp (startPos.x,startPos.x+2.6f,t);
			camera.transform.position = new Vector3 (x, camera.transform.position.y, camera.transform.position.z);
			yield return null;
		}

		yield break;
	}

	private IEnumerator ZoomIn(float duration) {
		var startPos = camera.transform.localPosition;
		var startSize = camera.orthographicSize;
		float t = 0;
		while (t < 1) {
			t += duration*Time.deltaTime;
			camera.orthographicSize = Mathf.Lerp (startSize,8.1f,t);
			var x = Mathf.Lerp (startPos.x,startPos.x-2f,t);
			camera.transform.position = new Vector3 (x, camera.transform.position.y, camera.transform.position.z);
			yield return null;
		}

		yield break;
	}

	private void HandleSpineEvent (TrackEntry trackEntry, Spine.Event e) {
		if (e.Data.Name == "Landing") {
			Director.CameraShake ();
		}
		if (e.Data.Name == "JumpEnded") {
			kingKong.AnimationState.SetAnimation (0, "Idle", true);
		}
		if (e.Data.Name == "Grab") {
			maleCharacter.gameObject.SetActive (false);
		}
		if (e.Data.Name == "GrabEnded") {
			kingKong.AnimationState.SetAnimation (0, "JumpAway", false);
			kingKongAnimationIsDone = true;
		}
	}

	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			mousePress = true;
		}
		var ratio = camera.orthographicSize / 8.1f;
		var reflectionY = 0.52f - camera.transform.localPosition.y * 0.12f - (1 - ratio) * 0.45f;
		var strength = 4 -2 * (reflectionY / 0.52f);
		MirrorQuad.GetComponent<MeshRenderer>().material.SetFloat ("_ReflectionY", reflectionY);
		MirrorQuad.GetComponent<MeshRenderer>().material.SetFloat ("_Strength", strength);
	}
}
