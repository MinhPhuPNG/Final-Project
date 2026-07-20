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
    private void ShowDialoguePrompt(string message)
    {
        DialogueManager dialogueManager = DialogueManager.EnsureInstance();
        if (dialogueManager != null)
        {
            dialogueManager.ShowDialogue("Alchemist", message);
        }
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

        if (potionID == 1 && StoryManager.Instance != null &&
            (StoryManager.Instance.currentQuestState == QuestState.AlchemyTutorial))
        {
            string msg = "Oh wow, it turned out well. I could've sworn I heard a - DAMN IT. The cauldrons messed up again. Sorry but I don't think you can join at the rate our equipment's blowing up, my sister was trying to start a club though. She'll be outside, near the graveyard. You should go talk to her.";
            ShowDialoguePrompt(msg);
            StoryManager.Instance.AdvanceQuest(QuestState.MeetOccult);
        }

        gameObject.SetActive(false);
    }
}