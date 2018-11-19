using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaleCharacterDrop : MonoBehaviour {

	private float gravity = -30;
	private bool isDropping;
	void Update () {
		if (!isDropping) return;
		gravity += 60*Time.deltaTime;
		gravity = Mathf.Clamp (gravity, -40, 40);
		transform.localEulerAngles += new Vector3 (0,0,-260)*Time.deltaTime;
		transform.localPosition += new Vector3 (-5,-gravity,0)*Time.deltaTime;
	}

	public void Drop() {
		gameObject.SetActive (true);
		isDropping = true;
	}
}
