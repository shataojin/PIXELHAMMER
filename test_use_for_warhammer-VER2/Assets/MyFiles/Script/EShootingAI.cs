//using System.Collections;
//using UnityEngine;
//using UnityEngine.UI;

//public class EShootingAI : MonoBehaviour
//{
//    [SerializeField] private GameObject enemyPrefab;
//    [SerializeField] private float moveSpeed = 1.0f;
//    [SerializeField] private float runSpeed = 3.0f;
//    [SerializeField] private int maxShootingPatternCalls = 2;
//    [SerializeField] private int enemyHP = 10;
//    [SerializeField] private Slider healthBarSlider; // Add this for the health bar slider
//    [SerializeField] private float healthBarDisplayTime = 3.0f; // Display time after hit
//    private float healthBarTimer = 0.0f;
//    private int shootingPatternCounter = 0;
//    private bool meleed = false;
//    private bool dodged = false;
//    private EnemyControl enemyControl;
//    private Transform playerTransform;
//    private PlayerDetection playerDetection;
//    private float number;

//    private void Start()
//    {
//        GameObject player = GameObject.FindGameObjectWithTag("Player");
//        if (player != null)
//        {
//            playerTransform = player.transform;
//            playerDetection = player.GetComponent<PlayerDetection>();
//            Debug.Log("Player object with tag 'Player' found.");
//        }
//        else
//        {
//            Debug.LogError("Player object with tag 'Player' not found.");
//        }

//        if (enemyPrefab != null)
//        {
//            enemyControl = enemyPrefab.GetComponent<EnemyControl>();
//        }
//        else
//        {
//            Debug.LogError("Enemy object with tag 'Enemy' not found.");
//        }

//        number = Random.Range(playerDetection.midDistance, playerDetection.farDistance);
//        Debug.Log(number);

//        meleed = false;

//        // Initialize health bar
//        if (healthBarSlider != null)
//        {
//            healthBarSlider.maxValue = enemyHP;
//            healthBarSlider.value = enemyHP;
//            healthBarSlider.gameObject.SetActive(false);
//        }
//        else
//        {
//            Debug.LogError("Health bar slider not assigned.");
//        }
//    }

//    private void Update()
//    {
//        UpdateHealthBarVisibility();
//        //// Ensure the health bar follows the enemy
//        //if (healthBarSlider != null)
//        //{
//        //    healthBarSlider.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 2, 1.5));
//        //}

//        // Pressure = 0 action
//        if (enemyControl.PressureLevel == 0)
//        {
//            // Set stay at far range to shoot
//            if (enemyControl.FarTrigger == true)
//            {
//                enemyControl.RandomStayRange(playerTransform, enemyPrefab.transform, moveSpeed, number);
//                Debug.Log(0);
//                enemyControl.ShootingPattern();
//            }
//            // Walk to player
//            else
//            {
//                enemyControl.WalkToPlayer(playerTransform, moveSpeed);
//            }
//        }
//        // Pressure = 1 action
//        else if (enemyControl.PressureLevel == 1)
//        {
//            // Set stay at far range to shoot
//            if (enemyControl.FarTrigger == true)
//            {
//                enemyControl.RandomStayRange(playerTransform, enemyPrefab.transform, runSpeed, number);
//                Debug.Log(1);
//                enemyControl.ShootingPattern();
//            }
//            // Run to player
//            else
//            {
//                enemyControl.RunToPlayer(playerTransform, runSpeed);
//            }
//        }
//        // Pressure = 2 action
//        else if (enemyControl.PressureLevel == 2)
//        {
//            // Set stay at far range to shoot counting shoots then move back to moving
//            if (enemyControl.FarTrigger == true && shootingPatternCounter < maxShootingPatternCalls)
//            {
//                enemyControl.RandomStayRange(playerTransform, enemyPrefab.transform, runSpeed, number);
//                Debug.Log(2);
//                enemyControl.ShootingPattern();
//                StartCoroutine(IncrementShootingPatternCounterWithDelay());
//            }
//            // Run to player if enemy not close range 
//            else if (enemyControl.CloseTrigger != true)
//            {
//                enemyControl.RunToPlayer(playerTransform, runSpeed);
//            }
//            if (enemyControl.CloseTrigger == true && meleed == false)
//            {
//                enemyControl.MeleePatter();
//                meleed = false;
//                // Note: Needs to be changed to surrender mode then attack or just direct attack
//            }
//        }
//        // Pressure over level action
//        else
//        {
//            Debug.LogError("Pressure over setting!!!!");
//        }
//        // Enemy action when in player close range and not in pressure = 2 
//        if (enemyControl.CloseTrigger == true && enemyControl.PressureLevel != 2)
//        {
//            // Random number for player action
//            int randomNumber = Random.Range(0, 2);
//            if (randomNumber == 0 && meleed == false)
//            {
//                enemyControl.MeleePatter();
//                Debug.Log("Test1 now!!!");
//                meleed = true;
//                dodged = true;
//            }
//            if (randomNumber == 1 && dodged == false)
//            {
//                enemyControl.DodgePatter();
//                Debug.Log("Test2 now!!!");
//                dodged = true;
//                meleed = true;
//            }
//        }
//        // When out of player close range reset for next trigger
//        if (enemyControl.CloseTrigger == false && enemyControl.PressureLevel != 2)
//        {
//            meleed = false;
//            dodged = false;
//        }
//    }

//    IEnumerator IncrementShootingPatternCounterWithDelay()
//    {
//        yield return new WaitForSeconds(2);
//        shootingPatternCounter++;
//        Debug.LogError("Delay shoot !!!");
//    }

//    private void UpdateHealthBarVisibility()
//    {
//        if (healthBarSlider != null && healthBarTimer > 0)
//        {
//            healthBarTimer -= Time.deltaTime;
//            if (healthBarTimer <= 0)
//            {
//                healthBarSlider.gameObject.SetActive(false); // Hide the health bar
//            }
//            else
//            {
//                healthBarSlider.gameObject.SetActive(true); // Show the health bar
//            }
//        }
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("PlayerBullet"))
//        {
//            TakeDamage(1); // Example damage value
//            Destroy(other.gameObject); // Remove Bullet
//        }
//    }

//    private void TakeDamage(int damage)
//    {
//        enemyHP -= damage;
//        if (healthBarSlider != null)
//        {
//            healthBarSlider.value = enemyHP;
//            healthBarTimer = healthBarDisplayTime; // Reset timer when hit
//        }
//        if (enemyHP <= 0)
//        {
//            Destroy(gameObject);
//        }
//    }
//}
