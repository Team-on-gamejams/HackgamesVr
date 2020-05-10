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

	int aliveEnemies;

	public void StartWave() {
		Wave wave = waves[currWave];
		aliveEnemies = wave.enemiesPrefab.Length;
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
		if(currWave >= waves.Length) {
			//TODO: win game
			currWave = waves.Length - 1;
		}
	}

	void SpawnEnemy(GameObject _enemy) {
		Enemy enemy = Instantiate(_enemy, spawnPoint.position, Quaternion.identity, null).GetComponent<Enemy>();
		enemy.mothership = mothership;
		enemy.player = player;
		enemy.onDie += OnEnemyDie;
	}

	void OnEnemyDie() {
		--aliveEnemies;

		if(aliveEnemies <= 0) {
			//TODO: win wave
			OnWinWave();
			StartWave();
			
		}
	}

	[System.Serializable]
	public class Wave {
		public string name;
		public GameObject[] enemiesPrefab;
		public float[] delays;
	}
}
