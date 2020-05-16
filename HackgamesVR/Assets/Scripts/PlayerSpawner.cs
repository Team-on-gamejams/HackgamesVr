using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour {
	[SerializeField] GameObject steamVRPlayer;
	[SerializeField] GameObject keyboardPlayer;

	void Awake() {
#if VR_VERSION
		Instantiate(steamVRPlayer, transform.position, transform.rotation, transform);
#else
		Instantiate(keyboardPlayer, transform.position, transform.rotation, transform);
#endif
	}
}
