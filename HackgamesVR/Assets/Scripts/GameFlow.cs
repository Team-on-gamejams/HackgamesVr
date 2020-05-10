using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlow : MonoBehaviour
{
	[Header("Refs")] [Space]
	[SerializeField] Spawner spawner;

	private void Start() {
		LeanTween.delayedCall(0.5f, spawner.StartWave);
	}
}
