using UnityEngine;
using TMPro;
public class Occult : InteractableNPC
{
    private void Start()
    {
        npcName = "???";
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

        switch (StoryManager.Instance.currentQuestState)
        {
            case QuestState.MeetOccult:
                dialogueManager.ShowDialogue(npcName, "Hello. I know what you want. I need you to help me first...");
                StoryManager.Instance.AdvanceQuest(QuestState.GardenHarvest);
                break;

            case QuestState.GardenHarvest:
                dialogueManager.ShowDialogue(npcName, "It's time, harvest me some ingredients... I advise you grab at least 8 of each plant.");
                break;

            default:
                dialogueManager.ShowDialogue(npcName, "the three of the 20th one ... he waits");
                break;
        }
    }
}