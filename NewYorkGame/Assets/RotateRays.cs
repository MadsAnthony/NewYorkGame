using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateRays : MonoBehaviour {
	
	void Update () {
		transform.localEulerAngles += new Vector3 (0,0,100)*Time.deltaTime;
	}
}
