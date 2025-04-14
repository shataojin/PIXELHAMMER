using UnityEngine;
using System.Collections.Generic;

public class PositionPriorityManager : MonoBehaviour
{
    public Transform target;
    public float radius = 2f;
    public int priorityZones = 5;
    public GameObject enemyPrefab; // Reference to the enemy prefab
    public List<Transform> enemies = new List<Transform>();
    private Dictionary<int, Vector3> priorityPositions = new Dictionary<int, Vector3>();

    void Start()
    {
        CalculatePriorityPositions();
    }

    void Update()
    {
        // Call this if target's position changes dynamically
        CalculatePriorityPositions();
    }

    public void CalculatePriorityPositions()
    {
        priorityPositions.Clear();
        float angleStep = 360f / priorityZones;
        for (int i = 1; i <= priorityZones; i++)
        {
            float angle = angleStep * (i - 1);
            Vector3 position = target.position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad)) * radius;
            priorityPositions[i] = position;
        }
    }

    public void InstantiateAndAssignEnemy()
    {
        GameObject newEnemy = Instantiate(enemyPrefab); // Instantiate the enemy prefab
        AssignPosition(newEnemy.transform); // Assign a priority position to the new enemy
    }

    public void AssignPosition(Transform enemy)
    {
        foreach (var position in priorityPositions)
        {
            if (!IsPositionOccupied(position.Value))
            {
                enemy.position = position.Value;
                enemies.Add(enemy); // Ensure enemy is tracked in the list
                return;
            }
        }
    }

    bool IsPositionOccupied(Vector3 position)
    {
        foreach (Transform enemy in enemies)
        {
            if (Vector3.Distance(enemy.position, position) < 0.1f)
            {
                return true;
            }
        }
        return false;
    }
}
