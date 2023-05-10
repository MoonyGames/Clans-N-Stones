using UnityEngine;
using System.Collections;

public class EnemyGenerator : MonoBehaviour
{
    public static EnemyGenerator Instance { get; private set; }

    public bool IsGenerating { get; set; } = true;

    [SerializeField] private GameObject _enemy;

    [SerializeField] private float _timeBetweenSpawn;

    [SerializeField] private Transform[] _spawnPoints;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    private void Start() => StartCoroutine(SpawnOnTimer());

    private IEnumerator SpawnOnTimer()
    {
        while (IsGenerating)
        {
            Instantiate(_enemy, _spawnPoints[Random.Range(0, _spawnPoints.Length)].position, Quaternion.identity);
            Instantiate(_enemy, _spawnPoints[Random.Range(0, _spawnPoints.Length)].position, Quaternion.identity);

            yield return new WaitForSeconds(_timeBetweenSpawn);
        }
    }
}
