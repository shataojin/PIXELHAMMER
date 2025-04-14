using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    [Header("Player Controller")]
    [SerializeField] private PlayerController playerController;

    private Slider healthBar;

    // Start is called before the first frame update
    void Start()
    {
        healthBar = GetComponent<Slider>();
        healthBar.value = playerController.currentHealth;
    }

    public void GetDamage(int damage)
    {
        playerController.currentHealth -= damage;

        if (playerController.currentHealth < 0)
        {
            playerController.currentHealth = 0;
        }

        UpdateHealthBar();
    }

    public void Heal(float healAmount)
    {
        playerController.currentHealth += healAmount;

        if (playerController.currentHealth > playerController.MaxHealth)
        {
            playerController.currentHealth = playerController.MaxHealth;
        }

        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        healthBar.value = playerController.currentHealth;
    }

}
