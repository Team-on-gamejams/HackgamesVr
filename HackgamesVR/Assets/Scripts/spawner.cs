using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
	[Header("Waves")] [Space]
	[SerializeField] Wave[] waves = null;
	[SerializeField] int currWave = 0;

	[Header("Refs")] [Space]
	[SerializeField] Transform spawnPoint;
	[SerializeField] Transform player;
	[SerializeField] Transform mothership;

	public void StartWave() {
		Wave wave = waves[currWave];

		StartCoroutine(SpawnRoutine());
		
		IEnumerator SpawnRoutine() {
			for (int i = 0; i < wave.enemiesPrefab.Length; ++i) {
				SpawnEnemy(wave.enemiesPrefab[i]);
				yield return new WaitForSeconds(wave.delays[i]);
			}
		}
	}

	public void OnWinWave() {
		++currWave;
	}

	void SpawnEnemy(GameObject _enemy) {
		Enemy enemy = Instantiate(_enemy, spawnPoint.position, Quaternion.identity, null).GetComponent<Enemy>();
		enemy.mothership = mothership;
		enemy.player = player;
	}

	[System.Serializable]
	public class Wave {
		public string name;
		public GameObject[] enemiesPrefab;
		public float[] delays;
	}
}
