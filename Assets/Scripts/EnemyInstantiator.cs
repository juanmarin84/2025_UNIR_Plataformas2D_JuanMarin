using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyInstantiator : MonoBehaviour
{
    public static EnemyInstantiator Instance;

    [SerializeField] GameObject enemyPrefab;
    [SerializeField] float timeBetweenEnemyInst = 5f;
    [SerializeField] int maxEnemies = 10;
    [SerializeField] List<Transform> enemySpawnPoints = new List<Transform>();

    private int currentEnemies = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(InstantiateEnemies());
    }

    IEnumerator InstantiateEnemies()
    {
        while (true)
        {
            if (currentEnemies < maxEnemies && enemySpawnPoints.Count > 0)
            {
                int pointIndex = Random.Range(0, enemySpawnPoints.Count);
                Transform spawnPoint = enemySpawnPoints[pointIndex];

                Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                currentEnemies++;
            }

            yield return new WaitForSeconds(timeBetweenEnemyInst);
        }
    }

    public void EnemyDied()
    {
        currentEnemies--;
    }

}
