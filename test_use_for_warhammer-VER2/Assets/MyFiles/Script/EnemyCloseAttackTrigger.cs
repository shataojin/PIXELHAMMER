using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCloseAttackTrigger : MonoBehaviour
{
    public EMeleeAI meleeAI;               // Reference to the enemy's AI
  
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered enemy attack range.");
            meleeAI?.MeleePattern(); // Trigger the enemy's melee attack
        }
    }
}
