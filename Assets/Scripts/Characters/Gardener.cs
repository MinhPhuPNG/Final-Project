using UnityEngine;
using TMPro;
public class Gardener : InteractableNPC
{
    private void Start()
    {
        npcName = "Gardener";
    }
    public override void Talk()
    {
        DialogueManager dialogueManager = DialogueManager.EnsureInstance();
        if (dialogueManager == null)
        {
            return;
        }

        if (StoryManager.Instance == null)
        {
            return;
        }
        ResourceCounter counter = FindFirstObjectByType<ResourceCounter>();

        switch (StoryManager.Instance.currentQuestState)
        {
            case QuestState.GoToGardeningClub:
                dialogueManager.ShowDialogue(npcName, "Hey! You here to help with the garden? Oh, want to join! I'm sorry but we've been undergoing some changes in leadership, but I could introduce you to the Alchemy Club! I'll need you to grab one of each plant first though.");
                StoryManager.Instance.AdvanceQuest(QuestState.GardeningClubTutorial);
                break;

            case QuestState.GardeningClubTutorial:
                if (counter.GetMushroomCount() >= 1 && 
                    counter.GetPurpleFlowerCount() >= 1 && 
                    counter.GetTreeShroomCount() >= 1)
                {
                    dialogueManager.ShowDialogue(npcName, "Awesome, you got them all! Could you do me a huge favor and deliver these to the Alchemy Club inside?");
                    StoryManager.Instance.AdvanceQuest(QuestState.AlchemyDelivery);
                }
                else
                {
                    dialogueManager.ShowDialogue(npcName, "Need help? Just go up to each plant and grab them, they won't bite I promise.");
                }
                break;

            default:
                dialogueManager.ShowDialogue(npcName, "I love plants :D");
                break;
        }
    }
}