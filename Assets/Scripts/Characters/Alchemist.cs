using UnityEngine;
using TMPro;
public class Alchemist : InteractableNPC
{
    private void Start()
    {
        npcName = "Alchemist";
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
            case QuestState.AlchemyDelivery:
                dialogueManager.ShowDialogue(npcName, "Hello, are those plants for me? Oh, you want to join the Alchemy Club? Hm, try making something with the cauldron in the front.");
                StoryManager.Instance.AdvanceQuest(QuestState.AlchemyTutorial);
                break;

            case QuestState.AlchemyTutorialComplete:
                if (counter.HasTutorialPotion())
                    {
                        dialogueManager.ShowDialogue(npcName, "Oh wow, it turned out well. I could've sworn I heard a - DAMN IT. The cauldrons messed up again. Sorry but I don't think you can join at the rate our equipment's blowing up, my sister was trying to start a club though. She'll be outside, near the graveyard. You should go talk to her.");
                        StoryManager.Instance.AdvanceQuest(QuestState.MeetOccult);
                    }
                else{
                    dialogueManager.ShowDialogue(npcName, "Go ahead and try to make something with the cauldron next to me.");
                }
                break;

            case QuestState.MeetOccult:
                dialogueManager.ShowDialogue(npcName, "My sister is outside, near the graveyard.");
                break;
                
            default:
                dialogueManager.ShowDialogue(npcName, "Damn it, everyone keeps breaking my cauldrons after one brew. Someone isn't cleaning up after themselves...");
                break;
        }
    }
}