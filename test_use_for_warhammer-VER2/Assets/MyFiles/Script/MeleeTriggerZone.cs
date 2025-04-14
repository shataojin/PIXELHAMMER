using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeTriggerZone : MonoBehaviour
{
    public PlayerController playerController;

    private void OnTriggerEnter(Collider other)
    {
        //// 检查进入触发器的物体是否为敌人
        //if (other.CompareTag("Enemy"))
        //{
        //    // 获取敌人的 EMeleeAI 组件
        //    EMeleeAI enemyAI = other.GetComponent<EMeleeAI>();

        //    if (enemyAI != null)
        //    {
        //        // 设置近战区域可用
        //        playerController.meleeZoneAble = true;

        //        // 为了确保当前敌人受到攻击，绑定回调
        //        playerController.OnMeleeAttack = enemyAI.TakeDamageFromPlayer;

        //        // 打印日志
        //       // Debug.Log("敌人进入触发区域: " + other.name);
        //    }
        //}
    }

    private void OnTriggerExit(Collider other)
    {
       
    }
}
