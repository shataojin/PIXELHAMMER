using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDControlle : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text consumableText;
    [SerializeField] private TMP_Text ammoText;

    [Header("Prefabs and Parent Transform")]
    [SerializeField] private GameObject consumablePrefab;
    [SerializeField] private GameObject ammoPrefab;
    [SerializeField] private Transform consumableimagesParent;
    [SerializeField] private Transform ammoimagesParent;

    [Header("Player Controller")]
    [SerializeField] private PlayerController playerController;

    private List<GameObject> ammoImages = new List<GameObject>();
    private List<GameObject> consumableImages = new List<GameObject>();
    private float previousAmmo;
    private float previousConsumables;
    public float AmmoGap = 1f;
    public float ConsumablesGap = 1f;

    private void Start()
    {
        if (!ValidateReferences()) return;

        previousAmmo = playerController.ammo;
        previousConsumables = playerController.consumables;

        UpdateAmmoDisplay();
        UpdateConsumableDisplay();
    }

    private void Update()
    {
        if (playerController == null) return;

        if (playerController.ammo != previousAmmo)
        {
            UpdateAmmoDisplay();
            previousAmmo = playerController.ammo;
        }

        if (playerController.consumables != previousConsumables)
        {
            UpdateConsumableDisplay();
            previousConsumables = playerController.consumables;
        }

        if (playerController.resetAmmo)
        {
            ResetAmmoDisplay();
            playerController.resetAmmo = false;
        }
    }

    private bool ValidateReferences()
    {
        if (playerController == null || consumableText == null || ammoText == null || consumablePrefab == null || ammoPrefab == null)
        {
            Debug.LogError("Please assign all public references in the inspector.");
            return false;
        }
        return true;
    }

    private void UpdateAmmoDisplay()
    {
        if (ammoText != null && playerController != null)
        {
            ammoText.text = $"Ammo: {playerController.ammo}";
        }

        while (ammoImages.Count > playerController.ammo)
        {
            RemoveAmmoImage();
        }

        while (ammoImages.Count < playerController.ammo)
        {
            AddAmmoImage();
        }
    }

    private void UpdateConsumableDisplay()
    {
        if (consumableText != null && playerController != null)
        {
            consumableText.text = $"Consumable: {playerController.consumables}";
        }
        while (consumableImages.Count > playerController.consumables)
        {
            RemoveConsumableImage();
        }

        while (consumableImages.Count < playerController.consumables)
        {
            AddConsumableImage();
        }
    }

    private void AddAmmoImage()
    {
        GameObject newAmmoImage = Instantiate(ammoPrefab, ammoimagesParent);
        newAmmoImage.transform.localPosition = new Vector3(ammoImages.Count * AmmoGap, 0, 0);
        ammoImages.Add(newAmmoImage);
    }

    private void RemoveAmmoImage()
    {
        if (ammoImages.Count == 0) return;
        Destroy(ammoImages[ammoImages.Count - 1]);
        ammoImages.RemoveAt(ammoImages.Count - 1);
    }

    private void ResetAmmoDisplay()
    {
        foreach (GameObject ammoImage in ammoImages)
        {
            Destroy(ammoImage);
        }

        ammoImages.Clear();
        UpdateAmmoDisplay();
    }

    private void AddConsumableImage()
    {
        GameObject newConsumableImage = Instantiate(consumablePrefab, consumableimagesParent);
        newConsumableImage.transform.localPosition = new Vector3(consumableImages.Count * ConsumablesGap, -1, 0);
        consumableImages.Add(newConsumableImage);
    }

    private void RemoveConsumableImage()
    {
        if (consumableImages.Count == 0) return;
        Destroy(consumableImages[consumableImages.Count - 1]);
        consumableImages.RemoveAt(consumableImages.Count - 1);
    }

    private void ResetConsumableDisplay()
    {
        foreach (GameObject consumableImage in consumableImages)
        {
            Destroy(consumableImage);
        }

        consumableImages.Clear();
        UpdateConsumableDisplay();
    }
}
