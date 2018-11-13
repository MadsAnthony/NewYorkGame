using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroView : MonoBehaviour {

	[SerializeField] private GameObject textBubble;
	private bool loadingScene;

	private void Start() {
		textBubble.SetActive(false);
	}

	void Update () {
		if (Input.GetMouseButtonUp (0)) {
			if (!textBubble.activeSelf) {
				textBubble.SetActive(true);
			} else if (!loadingScene) {
				SceneManager.LoadScene ("LevelScene");
				loadingScene = true;
			}
		}
	}
}
