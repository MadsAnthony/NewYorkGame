using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour {

	[SerializeField] private GameObject tile;

	private Vector3 spawnPosition;
	private 
	void Start () {
		spawnPosition = new Vector3 (-8,0,-8);

		var angle = 7;
		for (int i = 0; i < 5; i++) {
			SpawnOneStage (angle);
		}

		SpawnOneSide (new Vector2 (1,0), 0);
	}

	void SpawnOneStage(float inputAngle) {
		SpawnOneSide (new Vector2 (1,0), inputAngle);
		SpawnOneSide (new Vector2 (0,1), inputAngle);
		SpawnOneSide (new Vector2 (-1,0), inputAngle);
		SpawnOneSide (new Vector2 (0,-1), inputAngle);
	}

	void SpawnOneSide(Vector2 dir, float inputAngle) {
		var towerWidth = 15;
		var angle = inputAngle;
		var cosX = Mathf.Cos (angle * Mathf.Deg2Rad);
		var sinX = Mathf.Sin (angle * Mathf.Deg2Rad);
		for (int i = 0; i<=towerWidth; i++) {
			var tileObject = GameObject.Instantiate (tile);
			spawnPosition += new Vector3(dir.x*cosX, 1*sinX, dir.y*cosX);
			if (inputAngle>0.5f && (i == 0 || i == towerWidth)) {
				spawnPosition += new Vector3(-0.06f*dir.x, -0.07f, -0.06f*dir.y);
			}
			if (i == towerWidth) {
				angle = 0;
			}
			tileObject.transform.localEulerAngles = new Vector3 (angle*-dir.y, 0 ,angle*dir.x);
			tileObject.transform.localPosition = spawnPosition;

		}
	}

}
