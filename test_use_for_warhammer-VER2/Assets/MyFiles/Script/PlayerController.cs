using UnityEngine;
using UnityEngine.InputSystem;

//should use state machine? create too many scripts?
//maybe got too many bool now ;(

public class PlayerController : MonoBehaviour
{
    private Vector3 moveInput;
    //private Vector3 SavedmoveInput;
    private Rigidbody rb;
    private PlayerInput playerInput;
    public HealthBarController healthBarController;
    public ArmorBarController armorBarController;
    private SpriteRenderer spriteRenderer;

    [Header("Player Action Settings")]
    public float moveSpeed = 5f;
    public float RunSpeed = 10f;
    private float CurrnetSpeed;
    //public float jumpForce = 10f; // not been used
    //private bool isGrounded = true; // not been used
    private bool speedUp = false;
    private bool facingRight = true; // 玩家是否面向右
    [SerializeField] private float lastMoveTime = 0f;
    public float doubleTapTime = 0.2f; // 双击检测时间间隔
    public float idleTime = 5.0f; // 闲置时间为5秒
    [SerializeField] private float timer = 0.0f;
    private bool isIdle = false;
    private bool isRunEnd = false;
    //private bool isSliding = false;
    //private float slideStartTime;
    //private float slideDuration = 2f; // 2秒
    //private Vector3 slideStartPos;
    //private Vector3 slideEndPos;
    public bool isRunDoublClick = false;

    [Header("currentHealth Settings")]
    public float MaxHealth = 100f;
    public float currentHealth = 100f;
    public float HealthRegenRate = 5;
    public bool playerDied = false;
    private bool isTakeDamge = false;
    public float freezeTimer = 1.00f;
    private float freezeTimerCounter;
    private bool isGetPushed = false;//maybe no need for this
    [Header("armour Settings")]//TODO: add armour to hp stytem!!
    public int armourType = 0;
    public float MaxArmour = 50f;
    public float currentArmour = 50f;
    public float armourCoolDown = 4f;
    public float armourRegenRate = 0.1f;
    [Header("currentstamina Settings")]
    public float maxStamina = 100f;
    public float currentstamina = 100f;
    public float staminaCoolDown = 2f;
    public float staminaRegenRate = 50f;
    public float staminaConsumptionRate = 50f;
    // public float staminaJumpUse = 50f;
    public float staminaRegenTimer = 0f;
    [Header("gear Settings")]
    public float maxAmmo = 15;
    public float ammo;
    public bool resetAmmo = false;
    public int consumableType = 0;
    public float maxConsumables = 3;
    public float consumables;
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;   // 子弹 Prefab
    public Transform bulletSpawnPoint; // 子弹的生成位置
    private bool isReloadStopMove = false;
    private bool isReloading = false;
    private bool isFire = false;
    private bool isKeepFire = false;
    [Header("Dodge Setting")]
    public float dodgeDistance = 5.0f; // 躲避的距离
    public float dodgeDuration = 0.5f; // 躲避所需时间
    public float staminaDodgeUse = 20f;
    private bool isDodging = false;
    private Vector3 dodgeStartPosition;
    private Vector3 dodgeEndPosition;
    private float dodgeStartTime;
    [Header("Melee Settings")]
    public GameObject meleeZone;
    public bool meleeZoneAble = false;
    public bool meleeDMG = false;
    private bool isMelee = false;
    private float meleetimer = 0;
    public float meleemaxtimer = 0.3f;
    [Header("animator Setting")]
    private Animator animator;
    [Header("Player GameObject Setting")]
    public GameObject shadow;

    int trueCount = 0;


    // animator.SetTrigger
    // animator.SetBool

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = new PlayerInput();
        ammo = maxAmmo;
        consumables = maxConsumables;
        CurrnetSpeed = moveSpeed;
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component missing!");
        }
        isKeepFire = true;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        meleeZone.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        playerInput.Player.Move.performed += OnMovePerformed;
        playerInput.Player.Move.canceled += OnMoveCanceled;
        playerInput.Player.Run.performed += OnRunPerformed;
        playerInput.Player.Run.canceled += OnRunCanceled;
        //playerInputActions.Player.Jump.performed += OnJumpPerformed;
        playerInput.Player.Fire.performed += OnFirePerformed;
        playerInput.Player.Roll.performed += OnRollPerformed;
        playerInput.Player.Reload.performed += OnReloadPerformed;
        playerInput.Player.Melee.performed += OnMeleePerformed;
        playerInput.Player.SpeciaMelee.performed += OnSpeciaMeleePerformed;
        playerInput.Player.Enable();
    }

    private void OnDisable()
    {
        playerInput.Player.Move.performed -= OnMovePerformed;
        playerInput.Player.Move.canceled -= OnMoveCanceled;
        playerInput.Player.Run.performed -= OnRunPerformed;
        playerInput.Player.Run.canceled -= OnRunCanceled;
        //playerInputActions.Player.Jump.performed -= OnJumpPerformed;
        playerInput.Player.Fire.performed -= OnFirePerformed;
        playerInput.Player.Roll.performed -= OnRollPerformed;
        playerInput.Player.Reload.performed -= OnReloadPerformed;
        playerInput.Player.Melee.performed -= OnMeleePerformed;
        playerInput.Player.SpeciaMelee.performed += OnSpeciaMeleePerformed;
        playerInput.Player.Disable();
    }

    private void FixedUpdate()
    {
        if (!isDodging && !isGetPushed && !isRunEnd && !isReloading && !isMelee && !isTakeDamge &&
            !isFire && moveInput != Vector3.zero) // 如果玩家没有在重新加载并且不在近战攻击状态
        {
            MovePlayer();
        }
        if (isDodging||isGetPushed || isRunEnd || isReloading || isMelee || isTakeDamge || isFire || moveInput != Vector3.zero)
        {
            isIdle = false; timer = 0;
        }
        else
        {
            isIdle = true;
        }

        if (!speedUp && staminaRegenTimer >= staminaCoolDown)
        {
            IncreaseStamina();
        }
        else if (speedUp && currentstamina > 0)
        {

            DecreaseStamina();
        }

        if(isDodging&& isFire)
        {
            isDodging =false;
            isFire = false;
            MovePlayer();
        }
       
    }

    #region player movement action

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        if (playerDied == false)
        {
            Vector2 input = context.ReadValue<Vector2>();
            moveInput = new Vector3(input.x, 0, input.y);

            if (isRunDoublClick == true)
            {
                if (Time.time - lastMoveTime < doubleTapTime)
                {
                    if (currentstamina > 0)
                    {
                        speedUp = true;
                    }
                    else
                    {
                        speedUp = false;
                    }
                }
                lastMoveTime = Time.time;
            }
            //if(SavedmoveInput!= moveInput && speedUp== true)
            //{
            //    animator.CrossFade("RunTurn 0", 0.1f);
            //}

            //this code for duoble press runing
            //if (Time.time - lastMoveTime < doubleTapTime)
            //{
            //    if (currentstamina > 0)
            //    {
            //        speedUp = true;
            //    }
            //    else
            //    {
            //        speedUp = false;
            //    }
            //}
            //lastMoveTime = Time.time;

            //need to think about this
            //if(SavedmoveInput!=moveInput && speedUp ==true)
            //{
            //    animator.SetTrigger("RunStopTurn");
            //}
            //if(SavedmoveInput.x != moveInput.x && SavedmoveInput.x !=0 && speedUp==true) //could works
            //{
            //    animator.SetBool("RunTurn", true);
            //}


        }
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        //if (moveInput.x != 0)
        //{
        //    SavedmoveInput = moveInput.normalized; // 归一化防止速度不同
        //}

        moveInput = Vector3.zero;

        if (speedUp == true)
        {
            animator.SetTrigger("RunEnd");
            //StartRunStopSlide();
            isRunEnd = true;
            freezeTimerCounter = 0;
        }

        speedUp = false;
    }

    private void OnRollPerformed(InputAction.CallbackContext context)
    {
        if (!isDodging && currentstamina > staminaDodgeUse && playerDied == false)
        {
            if (moveInput != Vector3.zero)
            {
                StartDodge();  // 启动躲避移动

                if (moveInput.x > 0)  // 向右滚动
                {
                    animator.SetTrigger("Rollright");
                }
                else if (moveInput.x < 0)  // 向左滚动
                {
                    animator.SetTrigger("Rollleft");
                }
                else if (moveInput.z > 0)  // 向上（前）滚动
                {
                    animator.SetTrigger("Rollup");
                }
                else if (moveInput.z < 0)  // 向下（后）滚动
                {
                    animator.SetTrigger("Rolldown");
                }
                DecreaseStaminaByDodge();  // 消耗耐力
            }
        }
    }

    private void OnRunPerformed(InputAction.CallbackContext context)
    {
        if (currentstamina > 0 && isRunDoublClick == false)
        {
            speedUp = true;
        }
    }

    private void OnRunCanceled(InputAction.CallbackContext context)
    {
        if (speedUp == true && isRunDoublClick == false)
        {
            speedUp = false;
        }

    }

    private void MovePlayer()
    {

        Vector3 move = moveInput * CurrnetSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }//checked

    private void CheckPlayerDirection()
    {
        if (playerDied == false)
        {
            // 使用输入来判断玩家面向的方向
            float move = moveInput.x;
            if (move > 0 && !facingRight)
            {
                Flip();
            }
            else if (move < 0 && facingRight)
            {
                Flip();
            }
        }
    }//checked

    private void Flip()
    {
        facingRight = !facingRight;
        spriteRenderer.flipX = !facingRight;
    }//checked problem with bullets

    public void StartDodge()
    {
        Vector3 direction = moveInput.normalized;  // 获取滚动的方向
        float adjustedDodgeDistance = dodgeDistance;

        // 如果滚动方向是上或下，则速度减半，并禁用影子
        if (moveInput.z > 0 || moveInput.z < 0)
        {
            adjustedDodgeDistance *= 0.5f;  // 将滚动距离减半
            shadow.SetActive(false);  // 禁用影子
        }

        dodgeStartPosition = transform.position;
        dodgeEndPosition = transform.position + direction * adjustedDodgeDistance;  // 根据调整后的距离设置躲避终点
        dodgeStartTime = Time.time;

        isDodging = true;

    }//checked

    public void PerformDodge()
    {
        float elapsed = Time.time - dodgeStartTime;
        float t = elapsed / dodgeDuration;

        // 使用 Lerp 计算当前位置
        transform.position = Vector3.Lerp(dodgeStartPosition, dodgeEndPosition, t);

        // 检查是否完成躲避
        if (t >= 1.0f)
        {
            isDodging = false;
            RollFinished();  // 滚动完成，恢复状态
        }
    }//checked

    public void RollFinished()
    {
        shadow.SetActive(true);
        Debug.Log("Roll finished.");
    }//checked

    //private void StartRunStopSlide()
    //{
    //    if (SavedmoveInput != Vector3.zero)
    //    {
    //        isSliding = true;
    //        slideStartTime = Time.time;
    //        slideStartPos = transform.position;
    //        slideEndPos = slideStartPos + SavedmoveInput * (RunSpeed * 2f); // 滑行2秒的距离
    //    }
    //}

    //private void PerformRunStopSlide()
    //{
    //    if (!isSliding) return;

    //    float elapsed = Time.time - slideStartTime;
    //    float t = elapsed / slideDuration;

    //    transform.position = Vector3.Lerp(slideStartPos, slideEndPos, t); // 平滑滑行

    //    if (t >= 1f)
    //    {
    //        isSliding = false;
    //        moveInput = Vector3.zero;
    //        animator.SetBool("Idle", true);
    //    }
    //}


    #endregion

    #region player fight action

    private void OnFirePerformed(InputAction.CallbackContext context)
    {
        if (ammo > 0 && playerDied == false)
        {
            if (!isFire)
            {
                isFire = true;
                moveInput = Vector3.zero;  // 停止移动
                animator.SetTrigger("Shoot");
            }
            if (!isKeepFire)
            {
                isKeepFire = true;
                moveInput = Vector3.zero;  // 停止移动
                animator.SetTrigger("KeepShoot");
            }

        }
    }

    private void OnReloadPerformed(InputAction.CallbackContext context)
    {
        if (playerDied == false && !isReloading)
        {
            isReloading = true;
            animator.SetTrigger("Reload");
            isReloadStopMove = false;
        }
    }

    private void OnMeleePerformed(InputAction.CallbackContext context)
    {

        if (!isMelee && playerDied == false)
        {
            animator.SetTrigger("Melee");
        }
        isMelee = true;
        meleetimer = 0;
        moveInput = Vector3.zero;  // 停止移动

        if (meleeDMG == true)
        {
            meleeDMG = false;
        }

    }

    private void OnSpeciaMeleePerformed(InputAction.CallbackContext context)
    {
        if (!isMelee && playerDied == false)
        {
            animator.SetTrigger("SpecialMelee");
        }
        isMelee = true;
        meleetimer = 0;
        moveInput = Vector3.zero;  // 停止移动

        if (meleeDMG == true)
        {
            meleeDMG = false;
        }
    }

    public void ShootBall()
    {
        if (ammo > 0)
        {
            RemoveAmmo();
            Shoot();
            Debug.Log("ShootBall ");
        }

    }//checked

    private void Shoot()
    {
        // 实例化子弹
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

        // 设置子弹的初始方向
        PlayerBullet bulletScript = bullet.GetComponent<PlayerBullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDirection(facingRight ? Vector3.right : Vector3.left);
        }
    }//checked

    public void FireDone()
    {
        isFire = false;
        Debug.Log("FireDone ");
    }//checked

    public void AbleKeepShoot()
    {
        isKeepFire = false;
        Debug.Log(" AbleKeepShoot" + isKeepFire);
    }//checked

    public void FinishReloading()
    {
        ReloadAmmo();
        isReloading = false; // 重置isReloading标志
        Debug.Log("Reload finished, isReloading: " + isReloading);

    }//checked



    #endregion

    #region Related numerical settings
    public void DecreaseStamina()
    {
        currentstamina -= staminaConsumptionRate * Time.deltaTime;
        if (currentstamina < 0)
            currentstamina = 0;
    }//checked

    public void IncreaseStamina()
    {
        currentstamina += staminaRegenRate * Time.deltaTime;
        if (currentstamina > maxStamina)
            currentstamina = maxStamina;
    }//checked

    public void DecreaseStaminaByDodge()
    {
        currentstamina -= staminaDodgeUse;
        if (currentstamina < 0)
            currentstamina = 0;
    }//checked

    public void RemoveAmmo()
    {
        if (ammo > 0)
        {
            ammo--;
        }
    }//checked

    public void RemoveConsumable()
    {
        if (consumables > 0)
        {
            consumables--;
        }
    }// not been used

    public void ReloadAmmo()
    {
        if (ammo != maxAmmo)
        {
            ammo = maxAmmo;
            resetAmmo = true;
        }
    }//checked

    public void ReloadConsumable()
    {
        if (consumables != maxConsumables)
        {
            consumables = consumables + 1;
        }
    }//checked

    #endregion

    #region code that not been used or for test
    //private void OnJumpPerformed(InputAction.CallbackContext context)
    //{
    //    if (isGrounded && currentstamina > 0)
    //    {
    //        Jump();
    //        DecreaseStaminaByJump();
    //        staminaRegenTimer = 0f;
    //    }
    //}



    //private void Jump()
    //{
    //    if (isGrounded && currentstamina > 0)
    //    {
    //        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    //        DecreaseStaminaByJump();
    //        isGrounded = false;
    //    }
    //}

    //public void DecreaseStaminaByHalf()
    //{
    //    currentstamina -= staminaJumpUse;
    //    if (currentstamina < 0)
    //        currentstamina = 0;
    //}

    //public void DecreaseStaminaByJump()
    //{
    //    currentstamina -= 20f; // Adjust currentstamina cost for jumping as needed
    //    if (currentstamina < 0)
    //        currentstamina = 0;
    //}

    //public void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Ground"))
    //    {
    //        isGrounded = true;
    //    }
    //}
    #endregion



    public void meleeDid()
    {
        Debug.Log("melee!!");
        meleeZone.gameObject.SetActive(true);
    }
    public void meleeDone()
    {
        Debug.Log("meleeDone!!");
        meleeZone.gameObject.SetActive(false);
    }

    public void PlayerTakeDmg()
    {
        if (isTakeDamge == false)
        {
            if (currentArmour > 5)
            {
                armorBarController.TakeDamage(10);
            }
            else if (currentArmour <= 1 && currentHealth > 0)
            {
                armorBarController.TakeDamage(10);
                healthBarController.GetDamage(10);
            }
            animator.SetTrigger("Hit");
            freezeTimerCounter = 0;

            isTakeDamge = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyAttackArea") && currentHealth>0)
        {
            PlayerTakeDmg();
        }
    }

    private void Update()
    {

        CheckPlayerDirection();

        if (moveInput == Vector3.zero && speedUp == false)
        {
            animator.SetBool("Idle", true);
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);
            CurrnetSpeed = moveSpeed;
        }
        else if (moveInput != Vector3.zero && speedUp == false)
        {
            animator.SetBool("Idle", false);
            animator.SetBool("Walk", true);
            animator.SetBool("Run", false);
            CurrnetSpeed = moveSpeed;
        }
        else if (moveInput != Vector3.zero && speedUp == true)
        {
            animator.SetBool("Idle", false);
            animator.SetBool("Walk", false);
            animator.SetBool("Run", true);
            CurrnetSpeed = RunSpeed;
        }

        //idle emo
        if (isIdle == true && !playerDied)
        {
            timer += Time.deltaTime;

            // 当计时器超过设定的闲置时间时，触发Idle状态
            if (timer >= idleTime)
            {
                animator.SetTrigger("EMO1");
                Debug.Log("Player is idle for 5 seconds");
                timer = 0;
            }
            if (moveInput != Vector3.zero)
            {
                isIdle = false;
                if (speedUp)
                {
                    animator.CrossFade("Run", 0.1f); // 平滑切换到奔跑动画
                }
                else
                {
                    animator.CrossFade("Walk", 0.1f); // 平滑切换到行走动画
                }
            }
        }

        #region currentstamina states
        if (!speedUp && currentstamina < maxStamina)
        {
            staminaRegenTimer += Time.deltaTime;
        }
        else
        {
            staminaRegenTimer = 0;//reset timer for currentstamina reg
        }


        if (speedUp && currentstamina <= 0)
        {
            speedUp = false;
            CurrnetSpeed = moveSpeed;
        }
        #endregion

        // dodg reset
        if (isDodging)
        {
            PerformDodge();
        }

        // die triggered
        if (currentHealth <= 0)
        {
            if (playerDied == false)
            {
                animator.SetTrigger("PlayeDown");
                playerDied = true;
            }
            moveInput = Vector3.zero;
        }

        //melee anime ongoing
        if (isMelee)
        {

            //Debug.Log("timer" + meleetimer);
            if (meleetimer >= meleemaxtimer)
            {
                isMelee = false;
                animator.SetBool("isAttacking", false);
                animator.SetTrigger("GoToIdle");
            }
            else
            {
                meleetimer += Time.deltaTime;
                animator.SetBool("isAttacking", true);
            }

        }

        //relaoding ammo
        if (isReloading)
        {
            Debug.Log("moveInput: " + moveInput);
            rb.velocity = Vector3.zero;

            // 第一次进入 Reload 状态时，停止移动
            if (!isReloadStopMove)
            {
                isReloadStopMove = true;
                moveInput = Vector3.zero;
            }

            // 如果玩家在 Reload 期间按下了移动键，则中断 Reload 动画并切换到移动动画
            if (isReloadStopMove && moveInput != Vector3.zero)
            {
                isReloading = false;
                isReloadStopMove = false;

                // **中断 Reload 动画并切换到 Walk 或 Run**
                if (speedUp)
                {
                    animator.CrossFade("Run", 0.1f); // 平滑切换到奔跑动画
                }
                else
                {
                    animator.CrossFade("Walk", 0.1f); // 平滑切换到行走动画
                }

                Debug.Log("Reload cancelled due to movement");
            }
        }
        //if (isSliding)
        //{
        //    PerformRunStopSlide();
        //}
        if (isRunEnd)
        {
            Debug.Log("isRunEnd" + isRunEnd);
            freezeTimerCounter += Time.deltaTime;

            if (freezeTimerCounter < freezeTimer)
            {
                rb.velocity = Vector3.zero;
            }
            else
            {
                isRunEnd = false;
            }
        }




        #region test function use

        //take dmg test
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (currentHealth > 0)
            {
                // currentHealth -= 10;
                healthBarController.GetDamage(10);
                animator.SetTrigger("Hit");
                freezeTimerCounter = 0;
                isTakeDamge = true;

            }
        }

        if (isTakeDamge)
        {
            freezeTimerCounter += Time.deltaTime;

            if (freezeTimerCounter < freezeTimer)
            {
                rb.velocity = Vector3.zero;
            }
            else
            {
                isTakeDamge = false;
            }
        }

        // Players continue to play
        if (Input.GetKeyDown(KeyCode.Alpha2) && currentHealth <= 0)
        {
            currentHealth = MaxHealth;
            currentArmour = MaxArmour;
            animator.SetTrigger("Rise");
            healthBarController.Heal(MaxHealth);
            playerDied = false;
        }

        // player pushed by enemy
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            animator.SetTrigger("Pushed");
            freezeTimerCounter = 0;
            isGetPushed = true;
        }

        if (isGetPushed)
        {
            freezeTimerCounter += Time.deltaTime;

            if (freezeTimerCounter < freezeTimer)
            {
                rb.velocity = Vector3.zero;
            }
            else
            {
                isGetPushed = false;
            }
        }
        // take dmg to the armor
        if (Input.GetKeyDown(KeyCode.Alpha4) && currentArmour > 0)
        {
            armorBarController.TakeDamage(10);
        }
        // use consumables
        if (Input.GetKeyDown(KeyCode.Alpha5) && consumables > 0)
        {
            consumables = consumables - 1;
        }

        if (Input.GetKeyDown(KeyCode.Alpha6) && currentHealth > 0)
        {
            PlayerTakeDmg();
        }

        #endregion




    }

   
}
