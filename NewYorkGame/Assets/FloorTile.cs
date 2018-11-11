using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTile : MonoBehaviour {
	
	[SerializeField] private GameObject sideTile;

	public GameObject SideTile {get { return sideTile; }}
}
