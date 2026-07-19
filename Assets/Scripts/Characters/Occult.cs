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
        ResourceCounter counter = FindFirstObjectByType<ResourceCounter>();

        switch (StoryManager.Instance.currentQuestState)
        {
            case QuestState.MeetOccult:
                dialogueManager.ShowDialogue(npcName, "Hello. I know what you want. I need you to help me first...");
                StoryManager.Instance.AdvanceQuest(QuestState.NightShift);
                FindFirstObjectByType<NightSceneTrigger>().LoadNightScene();
                break;

            case QuestState.NightShift:
                dialogueManager.ShowDialogue(npcName, "It's time, harvest some ingredients... The exact amount we need should be written in a book I left in the hallway.");
                StoryManager.Instance.AdvanceQuest(QuestState.GardenHarvest);
                break;

            case QuestState.GardenHarvest:
                if (counter.GetMushroomCount() >= 5 && 
                    counter.GetPurpleFlowerCount() >= 5 && 
                    counter.GetTreeShroomCount() >= 6)
                {
                    dialogueManager.ShowDialogue(npcName, "This should be enough, go ahead and use the remaining cauldrons to brew what we need.");
                    StoryManager.Instance.AdvanceQuest(QuestState.PotionBrew);
                }
                else
                {
                    dialogueManager.ShowDialogue(npcName, "Hurry on now, this isn't enough.");
                }
                break;

            default:
                dialogueManager.ShowDialogue(npcName, "the three of the 20th one ... he waits");
                break;
        }
    }
}