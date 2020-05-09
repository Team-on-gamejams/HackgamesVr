using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{
    public enum SpawnState { SPAWNING, WAITING, COUNTING };

    [System.Serializable]
    public class Wave
    {
        public string name;
        public GameObject enemy;
        public int enemyCount;
        public float spawnDelay;
    }

    public Wave[] waves;
    private int nextWave = 0;
    public float waveDelay;
    private float waveCountdown;
    private float spawnCountdown;

    private SpawnState state = SpawnState.COUNTING;
    void Start()
    {
        waveCountdown = waveDelay;
    }

    void Update()
    {
        if (state == SpawnState.WAITING)
        {
            if (!EnemyIsAlive())
            {

            }
            else
                return;
        }

        if (waveCountdown <= 0)
        {
            if (state != SpawnState.SPAWNING)
            {
                StartCoroutine(SpawnWave(waves[nextWave]));
            }
        }
        else
        {
            waveCountdown -= Time.deltaTime;
        }
    }

    bool EnemyIsAlive()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy") == null)
        {
            return false;
        }
        return true;
    }

    IEnumerator SpawnWave(Wave _wave)
    {
        state = SpawnState.SPAWNING;
        for (int i = 0; i < _wave.enemyCount; i++)
        {
            SpawnEnemy(_wave.enemy);
            yield return new WaitForSeconds(1f / _wave.spawnDelay);
        }
        state = SpawnState.WAITING;
        yield break;
    }

    void SpawnEnemy(GameObject _enemy)
    {
        Instantiate(_enemy, transform.position, Quaternion.identity);
    }
}
