using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
	[Header("Waves")] [Space]
	[SerializeField] Wave[] waves = null;
	[SerializeField] int currWave = 0;

	[Header("Waves")] [Space]
	[SerializeField] Transform spawnPoint;

	void SpawnWave(Wave wave) {
		StartCoroutine(SpawnRoutine());
		
		IEnumerator SpawnRoutine() {
			for (int i = 0; i < wave.enemiesPrefab.Length; ++i) {
				SpawnEnemy(wave.enemiesPrefab[i]);
				yield return new WaitForSeconds(wave.delays[i]);
			}
		}
	}

	void SpawnEnemy(GameObject enemy) {
		Instantiate(enemy, spawnPoint.position, Quaternion.identity, null);
	}

	[System.Serializable]
	public class Wave {
		public string name;
		public GameObject[] enemiesPrefab;
		public float[] delays;
	}
}
