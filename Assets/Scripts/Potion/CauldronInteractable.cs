using UnityEngine;

public class CauldronInteractable : InteractableResource
{
    [Header("Quest Restrictions")]
    public QuestState activeQuestPhase;
    public bool lockAfterSingleUse = true;

    [Header("Exact Ingredient Costs")]
    public int requiredMushrooms = 0;
    public int requiredPurpleFlowers = 0;
    public int requiredTreeShrooms = 0;

    [Header("Feedback Effects & Spawning")]
    public GameObject potionPrefab;
    public Transform spawnPoint;

    private ResourceCounter playerInventory;
    private bool hasBrewed = false;

    void Start()
    {
        playerInventory = FindFirstObjectByType<ResourceCounter>();
        promptText = "Press E to Brew";
        destroyWhenEmpty = false;
    }

    public override void Interact()
    {
        if (hasBrewed && lockAfterSingleUse)
        {
            ShowDialoguePrompt("The cauldron has cooled, perhaps unusable after you have used it.");
            return;
        }

        if (StoryManager.Instance.currentQuestState != activeQuestPhase)
        {
            ShowDialoguePrompt("What are you doing, punk?");
            return;
        }

        bool hasEnoughMushrooms = playerInventory.GetMushroomCount() >= requiredMushrooms;
        bool hasEnoughFlowers = playerInventory.GetPurpleFlowerCount() >= requiredPurpleFlowers;
        bool hasEnoughTreeShrooms = playerInventory.GetTreeShroomCount() >= requiredTreeShrooms;

        if (hasEnoughMushrooms && hasEnoughFlowers && hasEnoughTreeShrooms)
        {
            playerInventory.ConsumeResources(requiredMushrooms, requiredPurpleFlowers, requiredTreeShrooms);
            TriggerBrewSuccess();
            hasBrewed = true;

            ShowDialoguePrompt("The brewing seems to work, make sure to pick up the bottle.");
        }
        else
        {
            string missingFeedback = "Somethings wrong... \n";
            if (requiredMushrooms > 0) missingFeedback += $"- {requiredMushrooms} Mushrooms\n";
            if (requiredPurpleFlowers > 0) missingFeedback += $"- {requiredPurpleFlowers} Purple Flowers\n";
            if (requiredTreeShrooms > 0) missingFeedback += $"- {requiredTreeShrooms} Tree Shrooms\n";

            ShowDialoguePrompt(missingFeedback);
        }
    }

    private void TriggerBrewSuccess()
    {
        if (potionPrefab != null && spawnPoint != null)
        {
            Instantiate(potionPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    private void ShowDialoguePrompt(string message)
    {
        DialogueManager dialogueManager = DialogueManager.EnsureInstance();
        if (dialogueManager != null)
        {
            dialogueManager.ShowDialogue("Cauldron", message);
        }
    }
}