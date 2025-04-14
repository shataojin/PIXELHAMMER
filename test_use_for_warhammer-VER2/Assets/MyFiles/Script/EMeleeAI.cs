using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.VersionControl.Asset;

public enum EnemyState
{
    LowPressure,
    MediumPressure,
    HighPressure,
    DefensivePattern,
    Charge
}

public class EMeleeAI : MonoBehaviour
{

    [SerializeField] private GameObject enemyPrefab;
    //[SerializeField] public GameObject player;
    [SerializeField] private float MoveSpeed = 1.0f;
    [SerializeField] private float RunSpeed = 3.0f;
    [SerializeField] private int enemyHP = 10;
    [SerializeField] public bool Meleed = false;
    //[SerializeField] private bool Doge = false;
    public EnemyControl enemyControl;
     public PlayerDetection playerDetection;
    public float number = 0;
    public bool moveAnimeSwitch = true;
    public bool moveAnimeON = false;
    public int aggroLevel = 0;
    public bool randomdone = false;
    private Animator animator;
    public float EMOStartTimer = 5f;
    public float EMOEndTimer = 10f;
    public bool TimerDone = false;
    public int hitAnimationCount = 3;
    public bool holdPosition = false;  // 新增标志位，控制敌人是否保持当前位置
    private Collider enemyCollider; // Reference to the collider
    public bool adajustUse = false;
    public bool EnmeyAggroLevelChecked= false;
    public EnemyState CurrentAggroLevel;
    public float randomFloat=0.1f;
    public GameObject AttackZone;
    public bool attatking = false;
    // animator.SetTrigger
    // animator.SetBool
    private void Start()
    {
        animator = GetComponent<Animator>();
        //player = GameObject.FindGameObjectWithTag("Player"); // 赋值给全局变量
        enemyCollider = GetComponent<Collider>();
        enemyControl = GetComponent<EnemyControl>();
        playerDetection = FindObjectOfType<PlayerDetection>();
        holdPosition = false;
       AttackZone.SetActive(false);

        // 初始化血条
        if (enemyControl.healthBarSlider != null)
        {
            enemyControl.healthBarSlider.maxValue = enemyHP;
            enemyControl.healthBarSlider.value = enemyHP;
            enemyControl.healthBarSlider.gameObject.SetActive(false);
        }

        if (enemyControl.playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                enemyControl.playerTransform = player.transform;
            }
            else
            {
                Debug.LogError("Player GameObject not found! Make sure the Player has the correct tag.");
            }
        }
    }

    void Update()
    {

        // when enter trigger range and enemy not reach set move point .enemy move action start
        if (enemyControl.FarTrigger == true && enemyControl.EnemyReachPoint == false)
        {
            moveAnimeON = true;
            number = Random.Range(playerDetection.closeDistance, playerDetection.midDistance);
            if (EnmeyAggroLevelChecked == false)
            {
                AggroLevelselect();
                EnmeyAggroLevelChecked = true;
            }
        }
        else if (enemyControl.FarTrigger == false)
        {
            number = 0;
            EnmeyAggroLevelChecked = false;
            moveAnimeON = false;
        }
        else if (enemyControl.EnemyReachPoint ==true)
        {
            moveAnimeON = false;
            if (enemyControl .EnemyAttacked== true)
            {
                if(attatking == false) { attatking = true; MeleePattern(); }
            }
        }

        //fix this shit
        //if (TimerDone == false && enemyControl.FarTrigger == false) { StartCoroutine(TimerCoroutine()); }
        //else { StopCoroutine(TimerCoroutine()); }


        enemyControl.UpdateHealthBarVisibility();
        enemyControl.EnemySideDetection();

        if (holdPosition == false)//this for disable moves but not animetion
        {
            EnemyActionSwitch();
            EnemyAggroActionList();
        }
       
            if (Input.GetKeyDown(KeyCode.L))
            {
                MeleePattern();
            }
 

    }

    public void EnemyActionSwitch()
    {
        //could use case switch im lazy
        if (moveAnimeON == false)
        {
            //idle
            animator.SetBool("Idle", true);
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);
        }
        else if (moveAnimeON == true && moveAnimeSwitch == true)
        {
            //walk
            animator.SetBool("Idle", false);
            animator.SetBool("Walk", true);
            animator.SetBool("Run", false);
        }
        else if (moveAnimeON == true && moveAnimeSwitch == false)
        {
            //run
            animator.SetBool("Idle", false);
            animator.SetBool("Walk", false);
            animator.SetBool("Run", true);
        }
    }
    public void EnemyAggroActionList()
    {

        if(CurrentAggroLevel == EnemyState.LowPressure)
        {
            if (enemyControl.CloseTrigger == true)
            {
                enemyControl.Surround(MoveSpeed);
                moveAnimeON = true;
                moveAnimeSwitch = true;
            }

            else if (enemyControl.FarTrigger == true)
            {
                enemyControl.WalkToPlayer(enemyControl.playerTransform, MoveSpeed);
                //moveAnimeON = true;
                //moveAnimeSwitch = true;
            }
        }
        else if (CurrentAggroLevel == EnemyState.MediumPressure)
        {
            if (enemyControl.MidTrigger == true)
            {
                CurrentAggroLevel = EnemyState.LowPressure;
            }
            else if (enemyControl.FarTrigger == true)
            {
                enemyControl.RunToPlayer(enemyControl.playerTransform, RunSpeed);
                moveAnimeON = true;
                moveAnimeSwitch = false;
            }
        }

        else if (CurrentAggroLevel == EnemyState.HighPressure)
        {
            if (enemyControl.CloseTrigger == true)
            {
                enemyControl.Surround(MoveSpeed);
                moveAnimeON = true;
                moveAnimeSwitch = true;

            }
            else if (enemyControl.FarTrigger == true)
            {
                enemyControl.RunToPlayer(enemyControl.playerTransform, RunSpeed);
                moveAnimeON = true;
                moveAnimeSwitch = false;
            }
        }

        else if (CurrentAggroLevel == EnemyState.DefensivePattern)
        {
            if (enemyControl.MidTrigger == true && enemyControl.CloseTrigger == false)
            {
                enemyControl.RandomStayRange(enemyControl.playerTransform, enemyPrefab.transform, MoveSpeed, number);
                moveAnimeON = true;
                moveAnimeSwitch = true;
            }
            else if (enemyControl.CloseTrigger == true)
            {
                enemyControl.Surround(RunSpeed);
                moveAnimeON = true;
                moveAnimeSwitch = true;

            }
            else if (enemyControl.FarTrigger == true)
            {
                enemyControl.WalkToPlayer(enemyControl.playerTransform, MoveSpeed);
                moveAnimeON = true;
                moveAnimeSwitch = true;
            }
        }

        else if (CurrentAggroLevel == EnemyState.Charge)
        {
             if (enemyControl.FarTrigger == true)
            {
                enemyControl.Surround(RunSpeed);
                moveAnimeON = true;
                moveAnimeSwitch = false;
            }
            // TODO:add another when take dmg change to medium pressure

        }
        else
        {
            Debug.LogError("pressure over setting!!!!");
        }


        #region old code


        ////pressure=0 action
        //if (aggroLevel== null)
        //{
        //    if (enemyControl.CloseTrigger == true)
        //    {
        //        enemyControl.Surround(MoveSpeed);
        //        //moveAnimeON = true;
        //        //moveAnimeSwitch = true;
        //    }

        //    else if (enemyControl.FarTrigger == true)
        //    {
        //        enemyControl.WalkToPlayer(playerTransform, MoveSpeed);
        //        //moveAnimeON = true;
        //        //moveAnimeSwitch = true;
        //    }


        //    ////set stay at far range to shoot
        //    //if (enemyControl.CloseTrigger==true)
        //    //{
        //    //    Debug.Log(0);
        //    //    //enemyControl.MeleePatter();
        //    //}
        //    ////walk to player
        //    //else
        //    //{
        //    //    enemyControl.WalkToPlayer(playerTransform, MoveSpeed);
        //    //}
        //}

        ////pressure=1 action
        //else if (aggroLevel == 1)
        //{
        //    if (enemyControl.MidTrigger == true)
        //    {
        //        aggroLevel = 0;

        //    }
        //    else if (enemyControl.FarTrigger == true)
        //    {
        //        enemyControl.RunToPlayer(playerTransform, RunSpeed);
        //        //moveAnimeON = true;
        //        //moveAnimeSwitch = false;
        //    }
        //    ////set stay at far range to shoot
        //    //if (enemyControl.FarTrigger == true)
        //    //{
        //    //    enemyControl.RandomStayRange(playerTransform, enemyPrefab.transform, RunSpeed, number);
        //    //    Debug.Log(1);
        //    //    enemyControl.ShootingPattern();
        //    //}
        //    ////run to player
        //    //else
        //    //{
        //    //    enemyControl.RunToPlayer(playerTransform, RunSpeed);
        //    //}
        //}
        ////pressure=2 action
        //else if (aggroLevel == 2)
        //{

        //    if (enemyControl.CloseTrigger == true)
        //    {
        //        enemyControl.Surround(MoveSpeed);
        //        //moveAnimeON = true;
        //        //moveAnimeSwitch = true;

        //    }
        //    else if (enemyControl.FarTrigger == true)
        //    {
        //        enemyControl.RunToPlayer(playerTransform, RunSpeed);
        //        //moveAnimeON = true;
        //        //moveAnimeSwitch = false;
        //    }
        //    ////set stay at far range to shoot counting shoots then move back to moving
        //    //if (enemyControl.FarTrigger == true && shootingPatternCounter < maxShootingPatternCalls)
        //    //{
        //    //    enemyControl.RandomStayRange(playerTransform, enemyPrefab.transform, RunSpeed, number);
        //    //    Debug.Log(2);
        //    //    enemyControl.ShootingPattern();

        //    //}
        //    ////run to player if enemy not close range 
        //    //else if (enemyControl.CloseTrigger != true)
        //    //{
        //    //    enemyControl.RunToPlayer(playerTransform, RunSpeed);
        //    //}
        //    //if (enemyControl.CloseTrigger == true && Meleed == false)
        //    //{
        //    //    enemyControl.MeleePatter();
        //    //    Meleed = false;
        //    //}

        //}
        ////pressure=3 action
        //else if (aggroLevel == 3)
        //{

        //    if (enemyControl.MidTrigger == true && enemyControl.CloseTrigger == false)
        //    {
        //        enemyControl.RandomStayRange(playerTransform, enemyPrefab.transform, MoveSpeed, number);
        //        moveAnimeON = true;
        //        moveAnimeSwitch = true;
        //    }
        //    else if (enemyControl.CloseTrigger == true)
        //    {
        //        enemyControl.Surround(MoveSpeed);
        //        moveAnimeON = true;
        //        moveAnimeSwitch = true;

        //    }
        //    else if (enemyControl.FarTrigger == true)
        //    {
        //        enemyControl.WalkToPlayer(playerTransform, MoveSpeed);
        //        moveAnimeON = true;
        //        moveAnimeSwitch = true;
        //    }
        //}
        ////pressure=4 action
        //else if (aggroLevel == 4)
        //{
        //    if (enemyControl.FarTrigger == true)
        //    {
        //        //charge attack needs some change
        //        enemyControl.RunToPlayer(playerTransform, RunSpeed);
        //        moveAnimeON = true;
        //        moveAnimeSwitch = false;
        //    }

        //    else if (enemyControl.CloseTrigger == true)
        //    {
        //        enemyControl.Surround(RunSpeed);
        //        moveAnimeON = true;
        //        moveAnimeSwitch = false;
        //    }
        //}
        // pressure over level action

        #endregion

    }
  
    public void AggroLevelselect()
    {
        EnemyState[] states = { EnemyState.LowPressure, EnemyState.MediumPressure,
            EnemyState.HighPressure,EnemyState.DefensivePattern,EnemyState.Charge};

        if (aggroLevel == 0)
        {
            CurrentAggroLevel = EnemyState.LowPressure;
        }
        else if (aggroLevel == 1)
        {
            CurrentAggroLevel = states[Random.Range(0, 2)]; // 0 到 1（LowPressure 或 MediumPressure）
        }
        else if (aggroLevel == 2)
        {
            CurrentAggroLevel = states[Random.Range(0, 3)]; // 0 到 2（LowPressure, MediumPressure, HighPressure）
        }
        else if (aggroLevel == 3)
        {
            CurrentAggroLevel = states[Random.Range(0, 4)]; // 0 到 3（前四个状态）
        }
        else if (aggroLevel >= 4)
        {
            CurrentAggroLevel = states[Random.Range(0, states.Length)]; // 0 到 4（全部状态）
        }

        Debug.Log("CurrentAggroLevel " + CurrentAggroLevel);
    }

    private IEnumerator TimerCoroutine()
    {
        TimerDone = true;

        yield return new WaitForSeconds(randomFloat); // 等待指定的秒数
        int randomValue = Random.Range(1, 4); // Generates a random number between 1 and 3 (4 is exclusive)
        Debug.Log("randomFloataaaa " + randomFloat);
        if (randomValue == 1)
        {
            animator.SetTrigger("Amused");
           
        }
         if (randomValue == 2)
        {
            animator.SetTrigger("Lauch");
            
        }
        if (randomValue == 3)
        {
            animator.SetTrigger("Tongue");
           
        }
        randomFloat = Random.Range(EMOStartTimer, EMOEndTimer);
        TimerDone = false;
    }// need be fix

private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet") && enemyHP > 0)
        {
            TakeDamage(1); // Example damage value
            Destroy(other.gameObject); // Remove Bullet
            int randomIndex = Random.Range(0, hitAnimationCount);
            animator.SetTrigger("Hit" + randomIndex);
        }
        if (other.CompareTag("PlayerMelee") && enemyHP > 0)
        {
            TakeDamage(1); // Example damage value
            int randomIndex = Random.Range(0, hitAnimationCount);
            animator.SetTrigger("Hit" + randomIndex);
        }

    }

   

    private void TakeDamage(int damage)
    {
        enemyHP -= damage;
        if (enemyControl.healthBarSlider != null)
        {
            enemyControl.healthBarSlider.value = enemyHP;
            enemyControl.healthBarTimer = enemyControl.healthBarDisplayTime; // Reset timer when hit
        }
        if (enemyHP <= 0)
        {
            StopAllCoroutines();
            animator.SetTrigger("Die");
            enemyControl.healthBarDisplayTime = 0;
            if (enemyCollider != null)
            {
                enemyCollider.enabled = false;
            }
        }
    }


    public void OnDieAnimationComplete()
    {
        Destroy(gameObject);
        Debug.Log("remove enemy");
        enemyControl.RemoveEnemy();
    }

    public void TakeDamgeStart()
    {
        holdPosition = true;
        Debug.Log("enemy stop");
    }

    public void TakeDamgeOver()
    {
        holdPosition = false;
        Debug.Log("enemy move");
    }

    public void EmoteOn()
    {
        holdPosition = true;
        Debug.Log("enemy stop");
    }

    public void EmoteOff()
    {        
        holdPosition = false;
        Debug.Log("enemy move");
    }

    public void QuickAtaackOn()
    {
        AttackZone.SetActive(true);
        holdPosition = true;
        Debug.Log("enemy quick attack");
    }

    public void QuickAtaackOff()
    {
        AttackZone.SetActive(false);
        holdPosition = false;
        Debug.Log("enemy stop");
        attatking = false;
    }

    //public void ShootingPattern()
    //{
    //    //needs to add fire rate
    //    Debug.Log("shooting now!!!");

    //}d

    public void MeleePattern()
    {
        //add attack speed base on PressureLevel
        Debug.Log("melee now!!!");
        animator.SetTrigger("QuickAttack");
    }

    public void DodgePattern()
    {
        //add  move enemy away from player
        Debug.Log("dodge now!!!");

    }


}


