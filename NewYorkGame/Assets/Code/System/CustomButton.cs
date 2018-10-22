using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomButton : MonoBehaviour {
	public Action OnClick;

	Collider collider;
	void Start() {
		collider = GetComponent<BoxCollider> ();
	}

	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			var mouseClickPos = Input.mousePosition;
			var mouseClickWorldPos = Camera.main.ScreenToWorldPoint (mouseClickPos);
			var width  = collider.bounds.size.x;
			var height = collider.bounds.size.y;
			//Debug.LogError (mouseClickWorldPos.x+" "+collider.bounds.center.x " "+width / 2f);

			if (mouseClickWorldPos.x > collider.bounds.center.x - width / 2f && mouseClickWorldPos.x < collider.bounds.center.x + width / 2f &&
				mouseClickWorldPos.y > collider.bounds.center.y - height  / 2f && mouseClickWorldPos.y < collider.bounds.center.y + height  / 2f) {
				if (OnClick != null) {
					OnClick ();
				}
			}
		}
	}

}
