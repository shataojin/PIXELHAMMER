using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBarController : MonoBehaviour
{
    public Image StaminaBar;  // Bind the circular stamina bar's Image component
    private PlayerController playerController;
    private float maxStamina;
    public float initialFillAmount = 0.6f;  // Initial fill amount

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        maxStamina = playerController.maxStamina; // Use maxStamina from PlayerController
        StaminaBar.fillAmount = initialFillAmount;
    }

    void Update()
    {
        UpdateStaminaBar();
    }

    public void UpdateStaminaBar()
    {
        float currentStamina = playerController.currentstamina;
        float fillAmount = (currentStamina / maxStamina) * initialFillAmount; // Scale by initialFillAmount
        StaminaBar.fillAmount = fillAmount;
    }
}
