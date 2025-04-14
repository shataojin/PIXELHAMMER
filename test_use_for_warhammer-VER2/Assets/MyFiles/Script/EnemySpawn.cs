using System.Collections;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private Transform playerTransform;
    [SerializeField] public float enemyXGap = 20f;
    [SerializeField] public float enemyZRandomRange = 5f;

    [SerializeField] private int maxEnemies = 10; // 最大敌人数
    public int currentenemy = 0;
    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            int currentEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
            currentenemy = currentEnemyCount;
            if (currentEnemyCount < maxEnemies)
            {
                SpawnEnemy();
            }
        }
    }

    private void SpawnEnemy()
    {
        Vector3 playerPosition = playerTransform.position;
        Vector3 spawnPosition = new Vector3(
            playerPosition.x + enemyXGap,
            -4f,
            playerPosition.z + Random.Range(-enemyZRandomRange, enemyZRandomRange)
        );

        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}
