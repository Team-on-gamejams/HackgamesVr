using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugHotkeys : MonoBehaviour {

	void Update() {
		if (Input.GetKeyDown(KeyCode.R)) {
			SceneManager.LoadScene(0);
		}
		else if (Input.GetKeyDown(KeyCode.Escape)) {
			QuitGame.QuitApp();
		}
	}
}
