using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelDatabase", menuName = "Level/New LevelDatabase", order = 1)]
public class LevelDatabase : ScriptableObject {
	public List<LevelAsset> levels = new List<LevelAsset>();
}
