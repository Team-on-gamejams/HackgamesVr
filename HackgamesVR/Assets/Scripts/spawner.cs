using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Spawner : MonoBehaviour {
	public int killedEnemies = 0;

	[Header("Waves")] [Space]
	[SerializeField] Wave[] waves = null;
	[SerializeField] int currWave = 0;

	[Header("Refs")] [Space]
	[SerializeField] Transform spawnPoint;
	[SerializeField] Transform player;
	[SerializeField] Transform mothership;
	[SerializeField] TextMeshProUGUI enemiesLeftTextField;

	int aliveEnemies;
	int killedEnemiesCurrWave;
	int spawnedEnemies;

	private void Awake() {
		enemiesLeftTextField.text = "  ";
	}

	public void StartWave() {
		Wave wave = waves[currWave];
		aliveEnemies = wave.enemiesPrefab.Length;
		killedEnemiesCurrWave = 0;
		StartCoroutine(SpawnRoutine());
		
		IEnumerator SpawnRoutine() {
			spawnedEnemies = 0;
			for (int i = 0; i < wave.enemiesPrefab.Length; ++i) {
				spawnedEnemies++;
				enemiesLeftTextField.text = $"Enemies left: {spawnedEnemies - killedEnemiesCurrWave}/{spawnedEnemies}";
				SpawnEnemy(wave.enemiesPrefab[i]);
				yield return new WaitForSeconds(wave.delays[i]);
			}
		}
	}

	public void OnWinWave() {
		++currWave;
		enemiesLeftTextField.text = $"  ";
		if (currWave >= waves.Length) {
			//TODO: win game
			currWave = waves.Length - 1;
			GameFlow.instance.OnWinGame();
		}
		else {
			GameFlow.instance.OnWinWave();
		}
	}

	public string GetProgressStr() {
		return $" ({currWave + 1}/{waves.Length})";
	}

	void SpawnEnemy(GameObject _enemy) {
		Enemy enemy = Instantiate(_enemy, spawnPoint.position, Quaternion.identity, null).GetComponent<Enemy>();
		enemy.mothership = mothership;
		enemy.player = player;
		enemy.onDie += OnEnemyDie;
	}

	void OnEnemyDie() {
		++killedEnemies;
		++killedEnemiesCurrWave;
		--aliveEnemies;

		enemiesLeftTextField.text = $"Enemies left: {spawnedEnemies - killedEnemiesCurrWave}/{spawnedEnemies}";

		if (aliveEnemies <= 0) {
			OnWinWave();
		}
	}

	[System.Serializable]
	public class Wave {
		public string name;
		public GameObject[] enemiesPrefab;
		public float[] delays;
	}
}
