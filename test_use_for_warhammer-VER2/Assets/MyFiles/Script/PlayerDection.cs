using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerDetection : MonoBehaviour
{
    // Setting the distances for enemy detection
    [SerializeField] public float closeDistance = 2.0f;
    [SerializeField] public float midDistance = 4.0f;
    [SerializeField] public float farDistance = 6.0f;
    [SerializeField] public List<GameObject> enemies = new List<GameObject>();
    public Dictionary<int, Vector3> priorityPositions = new Dictionary<int, Vector3>();
    public List<int> EnemyCounter = new List<int>();
    public Dictionary<int, List<GameObject>> zoneEnemies = new Dictionary<int, List<GameObject>>();
    public Dictionary<GameObject, int> enemyCurrentZones = new Dictionary<GameObject, int>();

    private Transform playerTransform;
    private EnemyControl enemyControl;
    public float radius = 1;
    public int priorityZones = 6;
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player object with tag 'Player' not found.");
        }

       

        // Add SphereColliders and set them as triggers
        AddSphereCollider(closeDistance, "Close");
        AddSphereCollider(midDistance, "Mid");
        AddSphereCollider(farDistance, "Far");

        InitializeEnemyCounter(priorityZones);
        //InitializeZoneEnemies(priorityZones);
    }

    private void Update()
    {
        CalculatePriorityPositions();
    }

    //build Enemy counter for each zone
    public void InitializeEnemyCounter(int size)
    {
        for (int i = 0; i <= size; i++)
        {
            EnemyCounter.Add(0);
        }
    }
    //build list for hold each zone enemy
    public void InitializeZoneEnemies(int size)
    {
        for (int i = 1; i <= size; i++)
        {
            zoneEnemies.Add(i, new List<GameObject>());
        }
    }

    //build collider info
    private void AddSphereCollider(float radius, string name)
    {
        GameObject child = new GameObject(name);
        child.transform.parent = transform;
        child.transform.localPosition = Vector3.zero;
        SphereCollider collider = child.AddComponent<SphereCollider>();
        collider.radius = radius;
        collider.isTrigger = true;
        child.AddComponent<RangeTrigger>().playerDetection = this;
    }
    //enter range
    public void OnTriggerEnterRange(string range, Collider other)
    {

        if (other.CompareTag("Enemy"))
        {
            EnemyControl enemyControl = other.GetComponent<EnemyControl>();
           // float distance = Vector3.Distance(transform.position, other.transform.position);
            //Debug.Log($"Enemy entered {range} range. Distance: {distance}");
            if (enemyControl != null)
            {
                if (range == "Close")
                {
                    enemyControl.CloseTrigger = true;
                }
                else if (range == "Mid")
                {
                    enemyControl.MidTrigger = true;
                }
                else if (range == "Far")
                {
                    enemyControl.FarTrigger = true;
                }
            }

            AddEnemy(other.gameObject);
            AssignEnemyToZone(other.gameObject);
        }

    }

    public void OnTriggerExitRange(string range, Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyControl enemyControl = other.GetComponent<EnemyControl>();
            //Debug.Log($"Enemy exited {range} range.");
            if (enemyControl != null)
            {
                if (range == "Close")
                {
                    enemyControl.CloseTrigger = false;
                }
                else if (range == "Mid")
                {
                    enemyControl.MidTrigger = false;
                }
                else if (range == "Far")
                {
                    enemyControl.FarTrigger = false;
                }
            }
            RemoveEnemy(other.gameObject);
            RemoveEnemyFromZone(other.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        // Draw detection range circles
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, closeDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, midDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, farDistance);

        DrawZones();
       // DrawRangeZones();
    }

    private void DrawZones()
    {
        Gizmos.color = Color.blue;
        float angleStep = 360f / priorityZones;
        for (int i = 0; i < priorityZones; i++)
        {
            float angle = angleStep * i;
            Vector3 startPosition = transform.position;
            Vector3 endPosition = transform.position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad)) * closeDistance;
            Gizmos.DrawLine(startPosition, endPosition);
        }
    }

    private void DrawRangeZones()
    {
        Gizmos.color = Color.red;
        float angleStep = 360f / priorityZones;
        for (int i = 0; i < priorityZones; i++)
        {
            float angle = angleStep * i + 30;
            Vector3 startPosition = transform.position;
            Vector3 endPosition = transform.position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad)) * closeDistance;
            Gizmos.DrawLine(startPosition, endPosition);
        }
    }

    public void AddEnemy(GameObject enemy)
    {
        if (enemy != null && !enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }
    }

    public void RemoveEnemy(GameObject enemy)
    {
        if (enemy != null && enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
        }
    }

    public void CalculatePriorityPositions()
    {
        priorityPositions.Clear();
        float angleStep = 360f / priorityZones;
        for (int i = 1; i <= priorityZones; i++)
        {
            float angle = angleStep * (i - 1);
            Vector3 position = playerTransform.position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad)) * radius;
            priorityPositions[i] = position;
            //Debug.Log($"Zone {i}: {position}");
        }
    }

    public void AssignEnemyToZone(GameObject enemy)
    {
        int newZoneIndex = GetClosestZoneIndex(enemy.transform.position);

        if (newZoneIndex != -1)
        {
            // 获取敌人当前所在的区域
            int currentZoneIndex;
            enemyCurrentZones.TryGetValue(enemy, out currentZoneIndex);

            // 如果敌人已经在当前区域，则不需要做任何事情
            if (currentZoneIndex == newZoneIndex) return;

            // 如果敌人之前在另一个区域，从该区域移除
            if (currentZoneIndex != -1 && zoneEnemies.ContainsKey(currentZoneIndex))
            {
                zoneEnemies[currentZoneIndex].Remove(enemy);
                EnemyCounter[currentZoneIndex]--;
                Debug.Log("remove from " + currentZoneIndex);
            }

            // 将敌人添加到新的区域
            if (!zoneEnemies.ContainsKey(newZoneIndex))
            {
                zoneEnemies[newZoneIndex] = new List<GameObject>();
            }

            zoneEnemies[newZoneIndex].Add(enemy);
            EnemyCounter[newZoneIndex]++;
            enemyCurrentZones[enemy] = newZoneIndex;
            Debug.Log("add to " + newZoneIndex);
        }
    }




    public void RemoveEnemyFromZone(GameObject enemy)
    {
        int zoneIndex;
        if (enemyCurrentZones.TryGetValue(enemy, out zoneIndex))
        {
            if (zoneEnemies.ContainsKey(zoneIndex) && zoneEnemies[zoneIndex].Contains(enemy))
            {
                zoneEnemies[zoneIndex].Remove(enemy);
                EnemyCounter[zoneIndex]--;
                Debug.Log("remove from " + zoneIndex);
            }
            enemyCurrentZones.Remove(enemy); // 移除敌人的当前区域记录
        }
    }



    public int GetClosestZoneIndex(Vector3 enemyPosition)
    {
        int closestZoneIndex = -1;
        float minDistance = float.MaxValue;

        foreach (var kvp in priorityPositions)
        {
            float distance = Vector3.Distance(enemyPosition, kvp.Value);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestZoneIndex = kvp.Key;
            }
        }

        return closestZoneIndex;
    }
}

//build each collider trigger
public class RangeTrigger : MonoBehaviour
{
    public PlayerDetection playerDetection;
    public string range;

    private void Start()
    {
        range = gameObject.name;
    }

    private void OnTriggerEnter(Collider other)
    {
        playerDetection.OnTriggerEnterRange(range, other);
    }

    private void OnTriggerExit(Collider other)
    {
        playerDetection.OnTriggerExitRange(range, other);
    }
}


