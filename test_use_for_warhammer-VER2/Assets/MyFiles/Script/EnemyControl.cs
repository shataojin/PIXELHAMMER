using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.VersionControl.Asset;

public class EnemyControl : MonoBehaviour
{
    [SerializeField] public GameObject player;
    public PlayerDetection playerDetection;
    public bool CloseTrigger = false, MidTrigger = false, FarTrigger = false, radomside=false;
    public int  level = 0, RandomNumber;
    public bool ReachMainSide = false;
    public bool sideNumberAdded = false;
    public bool fromOtherZone = false;
    public float EnemyRange = 1; // 敌人之间的间隔距离，可以根据需要在编辑器中调整
    public float enemyGap =1f;
    public bool randomRangeDone = false;
    float randomOffsetX;
    float randomOffsetZ;
    public Transform playerTransform;
    public bool facingRight = true;
    public Slider healthBarSlider; // Add this for the health bar slider
    public float healthBarTimer = 0.0f;
    public float healthBarDisplayTime = 3.0f; // Display time after hit
    public bool EnemyReachPoint = false;
    public float EnemyReachRange = 0.1f;
    public bool EnemyAttacked=false;

    private void Start()
    {
        CloseTrigger = false; MidTrigger = false; FarTrigger = false;
        player = GameObject.FindGameObjectWithTag("Player"); // 赋值给全局变量
        playerDetection = FindObjectOfType<PlayerDetection>();
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.LogError("Player GameObject not found! Make sure the Player has the correct tag.");
            }
        }
    }

   

    //敌人移动方式
    //walk
    public void WalkToPlayer(Transform playerTransform, float MoveSpeed)
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        transform.position += direction * MoveSpeed * Time.deltaTime;
    }
    //run
    public void RunToPlayer(Transform playerTransform, float MoveSpeed)
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        transform.position += direction * MoveSpeed * Time.deltaTime;
    }
    //给远程敌人随机等待位置
    //not use yet
    public void RandomStayRange(Transform playerTransform, Transform enemy, float moveSpeed, float number)
    {
        float distance = Vector3.Distance(playerTransform.position, enemy.position);

        if (distance < number)
        {
            Vector3 direction = (enemy.position - playerTransform.position).normalized;
            enemy.position += direction * moveSpeed * Time.deltaTime;
        }

        else if (distance > number)
        {
            Vector3 direction = (playerTransform.position - enemy.position).normalized;
            enemy.position += direction * moveSpeed * Time.deltaTime;
        }

    }



    //1:(+,0,0)
    //2:(+,0,+）
    //3:(-，0，+）
    //4:(-,0,0)
    //5:(-,0,-)
    //6:(+,0,-)
    //包围设定
    public void Surround(float MoveSpeed)
    {
        if (!radomside)
        {
            RandomNumber = Random.Range(0, 2);
            radomside = true;
        }

        if (playerDetection == null || playerDetection.enemies == null)
        {
            Debug.LogError("Surround has a problem: Check playerDetection and enemies list.");
            return;
        }

        // 获取最近的优先位置
        Vector3 nearestPoint = playerDetection.priorityPositions
            .Values
            .OrderBy(point => Vector3.Distance(transform.position, point))
            .FirstOrDefault();

        int nearestZoneIndex = playerDetection.priorityPositions
            .FirstOrDefault(p => p.Value == nearestPoint).Key;

        if (nearestZoneIndex == 0)
        {
            Debug.LogError("Failed to find the nearest zone index.");
            return;
        }

        // 更新敌人在每个区域的计数器
        if (!playerDetection.enemyCurrentZones.TryGetValue(gameObject, out int currentZoneIndex))
        {
            currentZoneIndex = -1;
        }

        bool isNewEnemy = currentZoneIndex != nearestZoneIndex;

        if (isNewEnemy)
        {
            if (currentZoneIndex != -1)
            {
                playerDetection.zoneEnemies[currentZoneIndex].Remove(gameObject);
                playerDetection.EnemyCounter[currentZoneIndex]--;
                randomRangeDone = false;
            }

            if (!playerDetection.zoneEnemies.ContainsKey(nearestZoneIndex))
            {
                playerDetection.zoneEnemies[nearestZoneIndex] = new List<GameObject>();
            }

            playerDetection.zoneEnemies[nearestZoneIndex].Add(gameObject);
            playerDetection.EnemyCounter[nearestZoneIndex]++;
            playerDetection.enemyCurrentZones[gameObject] = nearestZoneIndex;
            randomRangeDone = false;
        }

        // 检查如果nearestZoneIndex为1或4且敌人数量大于level ，则根据RandomNumber移动新来的敌人到其他ZONE
        if ((nearestZoneIndex == 1 || nearestZoneIndex == 4) && playerDetection.EnemyCounter[nearestZoneIndex] > level)
        {
            // 检查当前敌人是否已经在1或4区域
            if (playerDetection.zoneEnemies[nearestZoneIndex].IndexOf(gameObject) >= level)
            {
                int newZoneIndex;
                if (nearestZoneIndex == 1)
                {
                    newZoneIndex = (RandomNumber == 0) ? 2 : 6;
                }
                else
                {
                    newZoneIndex = (RandomNumber == 0) ? 3 : 5;
                }

                Vector3 newNearestPoint = playerDetection.priorityPositions[newZoneIndex];

                // 更新敌人在新区域的计数器
                playerDetection.zoneEnemies[nearestZoneIndex].Remove(gameObject);
                playerDetection.EnemyCounter[nearestZoneIndex]--;
                randomRangeDone = false;

                if (!playerDetection.zoneEnemies.ContainsKey(newZoneIndex))
                {
                    playerDetection.zoneEnemies[newZoneIndex] = new List<GameObject>();
                }
                playerDetection.zoneEnemies[newZoneIndex].Add(gameObject);
                playerDetection.EnemyCounter[newZoneIndex]++;
                playerDetection.enemyCurrentZones[gameObject] = newZoneIndex;
                randomRangeDone = false;
                nearestZoneIndex = newZoneIndex;
                nearestPoint = newNearestPoint;
            }
        }

        // 计算敌人的排列位置，并添加随机性
        Vector3 targetPosition = nearestPoint;
        List<GameObject> zoneEnemies = playerDetection.zoneEnemies[nearestZoneIndex];
        int enemyIndex = zoneEnemies.IndexOf(gameObject);

        // 随机偏移量
        if (randomRangeDone == false)
        {
            randomRangeDone=true;
             randomOffsetX = Random.Range(-enemyGap, enemyGap);
             randomOffsetZ = Random.Range(-enemyGap, enemyGap);
        }

        switch (nearestZoneIndex)//HashSet<int>?? 
        {
            case 1:
                targetPosition += new Vector3(enemyIndex * EnemyRange + randomOffsetX, 0,  randomOffsetZ);
                break;
            case 2:
                targetPosition += new Vector3(enemyIndex * EnemyRange + randomOffsetX, 0,  randomOffsetZ);
                break;
            case 3:
                targetPosition += new Vector3(-enemyIndex * EnemyRange - randomOffsetX, 0,  randomOffsetZ);
                break;
            case 4:
                targetPosition += new Vector3(-enemyIndex * EnemyRange - randomOffsetX, 0, randomOffsetZ);
                break;
            case 5:
                targetPosition += new Vector3(-enemyIndex * EnemyRange - randomOffsetX, 0, -randomOffsetZ);
                break;
            case 6:
                targetPosition += new Vector3(enemyIndex * EnemyRange + randomOffsetX, 0, - randomOffsetZ);
                break;
            default:
                Vector3 directionToCenter = (nearestPoint - transform.position).normalized;
                Vector3 offset = Vector3.Cross(directionToCenter, Vector3.up) * (enemyIndex - (zoneEnemies.Count - 1) / 2.0f) * EnemyRange;
                offset += new Vector3(randomOffsetX, 0, randomOffsetZ);
                targetPosition += offset;
                break;
        }

        // 避免区域数字小于0
        if (playerDetection.EnemyCounter[nearestZoneIndex] < 0)
        {
            playerDetection.EnemyCounter[nearestZoneIndex] = 0;
        }

        // 移动敌人
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) <= EnemyReachRange)
        {
            EnemyReachPoint = true;
        }
        else
        {
            EnemyReachPoint = false;
        }

        // 添加攻击判定逻辑：到达位置 + 在Zone 1 或 4 + 是该Zone的第一个敌人
        if (EnemyReachPoint /*&& (nearestZoneIndex == 1 || nearestZoneIndex == 4)*/)
        {
            if (zoneEnemies.Count > 0 && zoneEnemies[0] == gameObject)
            {
                EnemyAttacked = true;
            }
          
        }
    }



    public void RemoveEnemy()
    {
        if (playerDetection != null)
        {
            // 从 `enemies` 列表中移除
            playerDetection.RemoveEnemy(gameObject);

            // 从 `enemyCurrentZones` 字典中移除
            if (playerDetection.enemyCurrentZones.ContainsKey(gameObject))
            {
                int zoneIndex = playerDetection.enemyCurrentZones[gameObject];

                // 从 `zoneEnemies` 移除
                if (playerDetection.zoneEnemies.ContainsKey(zoneIndex))
                {
                    playerDetection.zoneEnemies[zoneIndex].Remove(gameObject);
                    playerDetection.EnemyCounter[zoneIndex]--; // 计数器减少
                }

                playerDetection.enemyCurrentZones.Remove(gameObject);
            }
        }
    }


    #region check Enemys location to flip itself
    public void EnemySideDetection()
    {
        float direction = playerTransform.position.x - transform.position.x;
        if (direction > 0 && !facingRight)
        {
            Flip();
        }
        else if (direction < 0 && facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    { 
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    #endregion

    public void UpdateHealthBarVisibility()
    {
        if (healthBarSlider != null && healthBarTimer > 0)
        {
            healthBarTimer -= Time.deltaTime;
            if (healthBarTimer <= 0)
            {
                healthBarSlider.gameObject.SetActive(false); // Hide the health bar
            }
            else
            {
                healthBarSlider.gameObject.SetActive(true); // Show the health bar
            }
        }
    }

   

}