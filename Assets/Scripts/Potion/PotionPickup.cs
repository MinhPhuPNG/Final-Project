using UnityEngine;

public class PotionPickup : InteractableResource
{
    [Header("Potion Settings")]
    public int potionID = 1;

    void Start()
    {
        resourceName = "RedPotion";
        amountPerHarvest = 1;
        usesRemaining = 1;
        promptText = "Press E to pickup potion";
        destroyWhenEmpty = true;
    }

    public override void Interact()
    {
        PotionUIController uiController = FindFirstObjectByType<PotionUIController>();
        if (uiController != null)
        {
            uiController.RevealPotionImage(potionID);
        }

        gameObject.SetActive(false);
    }
}