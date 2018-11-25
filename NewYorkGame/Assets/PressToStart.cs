using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressToStart : MonoBehaviour {
	
	private float x = 0;
	private Vector3 startScale;

	private void Start() {
		startScale = transform.localScale;
	}

	void Update () {
		x += Time.deltaTime*5;
		transform.localScale = startScale+new Vector3(Mathf.Sin (x)*5f,Mathf.Sin (x)*5f,0);
	}

}
