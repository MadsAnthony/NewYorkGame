using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxCollectable : MonoBehaviour {

	float x = 0;
	void Update () {
		x += Time.deltaTime*5;
		transform.localPosition = new Vector3(transform.localPosition.x,Mathf.Sin (x)*0.2f,transform.localPosition.z);
	}
}
