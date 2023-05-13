using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyGenerator : MonoBehaviour
{
    public static EnemyGenerator Instance { get; private set; }

    public bool IsGenerating { get; set; } = true;

    [SerializeField] private GameObject _enemy;
    [SerializeField] private Transform[] _spawnPoints;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    public IEnumerator SpawnOnTimer(int wavesCount, int timeBetweenWaves)
    {
        for(int i = 0; i < wavesCount; i++)
        {
            if (IsGenerating)
            {
                GameObject enemy1 = Instantiate(_enemy, _spawnPoints[Random.Range(0, _spawnPoints.Length)].position, Quaternion.identity);
                GameObject enemy2 = Instantiate(_enemy, _spawnPoints[Random.Range(0, _spawnPoints.Length)].position, Quaternion.identity);

                yield return new WaitForSeconds(timeBetweenWaves);
            }
        }
    }
}
