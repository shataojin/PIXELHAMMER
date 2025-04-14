using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float speed = 10.0f;       // 子弹速度
    [SerializeField] private float lifeTime = 5.0f;     // 子弹的生命周期
    private Vector3 direction = Vector3.right;          // 默认方向为右
    public int BulletDamage=1;
    private void Start()
    {
        // 在指定时间后销毁子弹
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // 子弹向指定方向移动
        transform.Translate(direction * speed * Time.deltaTime);
    }

    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    // Check if the collided object is an enemy
    //    if (other.CompareTag("Enemy"))
    //    {
    //        //// Get the Enemy component from the collided object
    //        //Enemy enemy = other.GetComponent<Enemy>();
    //        //if (enemy != null)
    //        //{
    //        //    // Deal damage to the enemy
    //        //    enemy.TakeDamage(10);
    //        //}

    //        // Destroy the bullet
    //        Destroy(gameObject);
    //        //Destroy(other.gameObject);
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    // 检查是否离开DestroyWall
    //    if (other.CompareTag("PlayerBulletDestroyWall"))
    //    {
    //        Destroy(gameObject);
    //    }
    //}
}
