using UnityEngine;

public class PotionPickup : InteractableResource
{
    [Header("Potion Settings")]
    public int potionID = 1;

    void Start()
    {
        resourceName = (potionID == 1) ? "TutorialPotion" : "HealthPotion";
        amountPerHarvest = 1;
        usesRemaining = 1;
        promptText = "Press E to pickup";
        destroyWhenEmpty = true;
    }

    public override void Interact()
    {
        ResourceCounter counter = FindFirstObjectByType<ResourceCounter>();
        if (counter != null)
        {
            counter.AddResource(resourceName, amountPerHarvest);
        }

        PotionUIController uiController = FindFirstObjectByType<PotionUIController>();
        if (uiController != null)
        {
            uiController.RevealPotionImage(potionID);
        }

        if (potionID == 1 && StoryManager.Instance != null && StoryManager.Instance.currentQuestState == QuestState.AlchemyTutorial)
        {
            StoryManager.Instance.AdvanceQuest(QuestState.AlchemyTutorialComplete);
        }

        gameObject.SetActive(false);
    }
}