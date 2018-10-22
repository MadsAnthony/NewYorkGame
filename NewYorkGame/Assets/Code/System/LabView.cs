using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using UnityEngine.SceneManagement;

public class LabView : UIView {
	[SerializeField] private float LeftBorder;
	[SerializeField] private float RightBorder;

	[SerializeField] private Vector3 FollowObjectOffset;
	[SerializeField] private GameObject FollowObject;
	[SerializeField] private GameObject CameraPivot;

	[SerializeField] private Hero hero;
	[SerializeField] private Hero BallonMan;


	[SerializeField] private CustomButton labButton;
	[SerializeField] private SkeletonAnimation labBall;
	[SerializeField] private GameObject labBallPivot;
	[SerializeField] private CustomButton labButtonShrink;
	[SerializeField] private AnimationCurve shrinkCurve;

	protected override void OnStart () {
		labButton.OnClick += (() => { 
			labBallOpened = !labBallOpened;
			labBall.state.SetAnimation (0, labBallOpened? "Open" : "Animation", false);
			FollowObjectOffset = new Vector3(4,0,0);
			FollowObject = labBall.gameObject;
			BallonMan.StopMoving();
			BallonMan.gameObject.SetActive (false);
		});
		labButtonShrink.OnClick += (() => {
			if (isShrinking) return;
			StartCoroutine (ShrinkBall(3));
		});
		hero.StopMoving ();
	}

	bool labBallOpened = true;
	bool isShrinking = false;
	IEnumerator ShrinkBall(float duration) {
		isShrinking = true;
		float t = 0;
		var startScale = labBallPivot.transform.localScale;
		var endScale = Vector3.one*0.15f;
		bool sucess = false;
		while (true) {
			if (Input.GetMouseButton (0)) {
				t += (1 / duration) * Time.deltaTime;
			} else {
				t -= 2*(1 / duration) * Time.deltaTime;
			}
			if (t < 0) {
				break;
			}
			var evalT = shrinkCurve.Evaluate (Mathf.Clamp01 (t));
			labBallPivot.transform.localScale = startScale*(1-evalT) + endScale*evalT;
			if (t>=1) {
				sucess = true;
				break;
			}

			yield return null;
		}
		if (sucess) {
			labBallPivot.SetActive (false);
			hero.transform.position = labBallPivot.transform.position;
			hero.StartMoving ();
			yield return new WaitForSeconds (0.5f);
			FollowObjectOffset = Vector3.zero;
			FollowObject = hero.gameObject;
		}
		isShrinking = false;
	}

	void Update () {
		if (FollowObject != null) {
			Vector3 distance = Vector3.zero;
			distance = (FollowObjectOffset + FollowObject.transform.position) - CameraPivot.transform.position;
			CameraPivot.transform.position += Time.deltaTime * 3 * new Vector3 (distance.x, 0, 0);

			CameraPivot.transform.position = new Vector3 (Mathf.Clamp (CameraPivot.transform.position.x, LeftBorder, RightBorder), CameraPivot.transform.position.y, CameraPivot.transform.position.z);
		}

		if (hero.transform.position.x<-22) {
			hero.transform.position = new Vector3 (100,0,0);
			hero.StopMoving ();
			FollowObject = null;
			Director.TransitionManager.PlayTransition (() => {SceneManager.LoadScene ("WorldSelectScene");},0.1f,Director.TransitionManager.FadeToBlack(),Director.TransitionManager.FadeOut());
		}
	}
}
