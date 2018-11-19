using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyCamera : MonoBehaviour {

	[SerializeField] private GameObject camera;
	[SerializeField] private GameObject skyLine;

	private Vector3 skyLineStartPos;
	// Use this for initialization
	void Start () {
		skyLineStartPos = skyLine.transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3 (transform.position.x,camera.transform.position.y*2,transform.position.z);
		var skyLinePos = ((camera.transform.position.y - 15) / 40);
		skyLine.transform.localPosition = new Vector3(skyLine.transform.localPosition.x,skyLineStartPos.y+skyLinePos*-10,skyLine.transform.localPosition.z);
		skyLine.transform.localEulerAngles = camera.transform.localEulerAngles;
	}
}
