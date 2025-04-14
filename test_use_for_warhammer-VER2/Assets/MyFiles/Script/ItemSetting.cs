using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSetting : MonoBehaviour
{
    public int Items;
    public Color type1Color = Color.red;
    public Color type2Color = Color.green; 
    public Color type3Color = Color.blue;

    void Start()
    {
        GenerateRandomNumber();
    }

    void GenerateRandomNumber()
    {
        int randomNumber = Random.Range(1, 4); // Random.Range with int includes min and excludes max
        Debug.Log("Random Number: " + randomNumber);
        Items = randomNumber;
        ChangeColor();
    }

    void ChangeColor()
    {
        // Get the Renderer component of the GameObject
        Renderer renderer = GetComponent<Renderer>();

        // Change the color based on the item type
        switch (Items)
        {
            case 1:
                renderer.material.color = type1Color;
                break;
            case 2:
                renderer.material.color = type2Color;
                break;
            case 3:
                renderer.material.color = type3Color;
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ReloadConsumable();
            }
        }
    }
}
