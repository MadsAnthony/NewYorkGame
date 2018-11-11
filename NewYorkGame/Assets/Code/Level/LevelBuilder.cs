using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour {

	[SerializeField] private GameObject tile;
	[SerializeField] private GameObject spike;

	private Vector3 spawnPosition;

	private void Start () {
		spawnPosition = new Vector3 (-8,0,-8);

		var angle = 7;
		for (int i = 0; i < 5; i++) {
			SpawnOneStage (angle);
		}

		SpawnOneSide (new Vector2 (1,0), 0, 0);
	}

	private void SpawnOneStage(float inputAngle) {
		SpawnOneSide (new Vector2 (1,0), inputAngle, 0);
		SpawnOneSide (new Vector2 (0,1), inputAngle, 270);
		SpawnOneSide (new Vector2 (-1,0), inputAngle, 180);
		SpawnOneSide (new Vector2 (0,-1), inputAngle, 90);
	}

	private void SpawnOneSide(Vector2 dir, float inputAngle, float facingAngle) {
		var towerWidth = 15;
		var randomI = Random.Range (0+4, towerWidth);
		var angle = inputAngle;
		var cosX = Mathf.Cos (angle * Mathf.Deg2Rad);
		var sinX = Mathf.Sin (angle * Mathf.Deg2Rad);
		for (int i = 0; i<=towerWidth; i++) {
			var addPos = new Vector3 (dir.x * cosX, 1 * sinX, dir.y * cosX);
			spawnPosition += addPos;
			if (inputAngle>0.5f && (i == 0 || i == towerWidth)) {
				spawnPosition += new Vector3(-0.06f*dir.x, -0.07f, -0.06f*dir.y);
			}
			if (i == towerWidth) {
				angle = 0;
			}

			var angleV = new Vector3 (0, facingAngle, angle);

			var floor = SpawnFloor (spawnPosition, angleV, i == towerWidth);

			if (i == randomI) {
				SpawnSpike (floor, new Vector3 (0, 1, 0));
			}
		}
	}

	private GameObject SpawnFloor(Vector3 position, Vector3 angle, bool isLast) {
		var tileObject = GameObject.Instantiate (tile);
		tileObject.GetComponent<FloorTile> ().SideTile.SetActive (isLast);
		tileObject.transform.localEulerAngles = angle;
		tileObject.transform.localPosition = position;
		return tileObject;
	}

	private void SpawnSpike(GameObject floor, Vector3 position) {
		var spikeObject = GameObject.Instantiate (spike);
		spikeObject.transform.parent = floor.transform;
		spikeObject.transform.localPosition = position;
		spikeObject.transform.localEulerAngles = new Vector3 (0, 0, 0);
		spikeObject.transform.parent = floor.transform.parent;
	}

}
