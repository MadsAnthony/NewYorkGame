using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

	Camera camera;

	[SerializeField] bool IgnoreCameraMaterial;
	[SerializeField] private Hero hero;
	[SerializeField] private GameObject cameraParent;
	[SerializeField] private GameObject kingKong;
	public Material cameraMaterial;

	public AnimationCurve showLayerCurve;
	public AnimationCurve hideLayerCurve;

	Vector3[] angles = new Vector3[4] {	new Vector3(0,0,0),
										new Vector3(0,-90,0),
										new Vector3(0,180,0),
										new Vector3(0,90,0)
									  };

	// Use this for initialization
	void Start () {
		if (IgnoreCameraMaterial) return;
		cameraMaterial.SetFloat ("_Transparency", 0);
	}

	// Update is called once per frame
	void Update () {
		Vector3 distance = Vector3.zero;
		if (hero != null && Mathf.Abs(hero.MovingDir)>0) {
			if (changingDirIndex != hero.ChangingDirIndex) {
				SetCameraAngle (hero.ChangingDirIndex);
			}
			hero.spinePivot.transform.localEulerAngles = new Vector3 (hero.spinePivot.transform.localEulerAngles.x,cameraParent.transform.localEulerAngles.y,hero.spinePivot.transform.localEulerAngles.z);
			kingKong.transform.localEulerAngles = new Vector3 (kingKong.transform.localEulerAngles.x,cameraParent.transform.localEulerAngles.y,kingKong.transform.localEulerAngles.z);
			var heroLocalPos = cameraParent.transform.InverseTransformPoint (hero.transform.position);
			distance = (heroLocalPos - transform.localPosition);
			if (!isRotating) {
				distance += new Vector3 (-3 * hero.MovingDir, 0, 0);
			}
			transform.localPosition += Time.deltaTime * 7f * new Vector3 (distance.x, distance.y, 0);
			transform.position = new Vector3 (transform.position.x,Mathf.Clamp (transform.position.y,12,40),transform.position.z);
		}
	}

	private bool isRotating = false;
	private int changingDirIndex;
	private void SetCameraAngle(int index) {
		if (!isRotating) {
			changingDirIndex = index;
			StartCoroutine(RotateTo (cameraParent.transform, angles[index].y, 0.5f, (x)=>{isRotating = x;}));
		}
	}

	public void CameraShake() {
		StartCoroutine(Shake());
	}

	public bool IsLayerVisible() {
		return cameraMaterial.GetFloat("_Transparency")>0.1f;
	}
	public void ShowLayer() {
		StartCoroutine(ShowLayerCr(showLayerCurve));
	}

	public void HideLayer() {
		StartCoroutine(ShowLayerCr(hideLayerCurve));
	}

	IEnumerator ShowLayerCr(AnimationCurve animationCurve, float duration = 0.3f) {
		float t = 0;
		while (t<1) {
			t += (1 / ((duration))) * Time.deltaTime;

			float evalT = animationCurve.Evaluate (t);

			cameraMaterial.SetFloat ("_Transparency", evalT);

			yield return null;
		}
	}

	IEnumerator Shake() {
		float x = 0;
		while (true) {
			x += 0.4f;
			transform.localEulerAngles = new Vector3 (0,0,4*Mathf.Sin(x));
			if (x > 4) {
				break;
			}
			yield return null;
		}
		transform.localEulerAngles = new Vector3 (0,0,0);
	}

	// Find another place for this static method. Shouldn't exist in GameBoard.
	public static IEnumerator RotateTo(Transform transform, float toAngle, float time, Action<bool> callback = null) {
		if (callback != null) {
			callback (true);
		}

		float t = 0;
		var startAngle = transform.localEulerAngles;
		var angleA = (startAngle.y + 360) % 360;
		var angleB = (toAngle + 360) % 360;

		var dist = angleB-angleA;
		if (Mathf.Abs (dist)>180) {
			dist = -Mathf.Sign (dist) * (360 - Mathf.Abs (dist));
		}

		while (true) {
			t += 1/time*Time.deltaTime;

			transform.localEulerAngles = startAngle + Mathf.Clamp01(t) * new Vector3 (0, dist, 0);

			if (t >= 1) {
				break;
			}
			yield return null;
		}
		if (callback != null) {
			callback (false);
		}
	}

}
