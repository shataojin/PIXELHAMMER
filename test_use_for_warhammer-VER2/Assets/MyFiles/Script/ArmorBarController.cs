using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ArmorBarController : MonoBehaviour
{
    [Header("Player Controller")]
    [SerializeField] private PlayerController playerController;

    private Slider armorBar;
    private Coroutine restoreCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        if (playerController == null)
        {
            Debug.LogError("PlayerController is not assigned!");
            return;
        }

        armorBar = GetComponent<Slider>();

        if (armorBar == null)
        {
            Debug.LogError("Slider component is not found!");
            return;
        }

        armorBar.maxValue = playerController.MaxArmour;
        armorBar.value = playerController.currentArmour;
    }

    public void TakeDamage(int damage)
    {
        if (playerController == null) return;

        if (restoreCoroutine != null)
        {
            StopCoroutine(restoreCoroutine);
        }

        restoreCoroutine = StartCoroutine(DamageArmorSmoothly(damage));
    }

    private IEnumerator DamageArmorSmoothly(int damage)
    {
        float elapsedTime = 0f;
        float duration = 1f; // Adjust the speed of armor decrease as needed
        float startArmor = playerController.currentArmour;

        float targetArmor = Mathf.Max(playerController.currentArmour - damage, 0);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            playerController.currentArmour = Mathf.Lerp(startArmor, targetArmor, elapsedTime / duration);
            armorBar.value = playerController.currentArmour;
            yield return null;
        }

        playerController.currentArmour = targetArmor;
        armorBar.value = playerController.currentArmour;

        // Start restoring armor after damage is done
        restoreCoroutine = StartCoroutine(RestoreArmor());
    }

    private IEnumerator RestoreArmor()
    {
        
        yield return new WaitForSeconds(playerController.armourCoolDown);

        float elapsedTime = 0f;
        float duration = 1f; // Adjust the speed of armor restoration as needed

        float startArmor = playerController.currentArmour;

        float targetArmor = armorBar.maxValue;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            playerController.currentArmour = Mathf.Lerp(startArmor, targetArmor, elapsedTime / duration);
            armorBar.value = playerController.currentArmour;
            yield return null;
        }

        playerController.currentArmour = targetArmor;
        armorBar.value = playerController.currentArmour;

    }


}
