using UnityEngine;
using TMPro;
public class Counselor : InteractableNPC
{
    private void Start()
    {
        npcName = "Counselor";
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
            case QuestState.TalkToCounselor:
                dialogueManager.ShowDialogue(npcName, "Listen carefully. You need to join a student club immediately. The gardening club is outside if you want to try.");
                StoryManager.Instance.AdvanceQuest(QuestState.GoToGardeningClub);
                break;

            case QuestState.GoToGardeningClub:
                dialogueManager.ShowDialogue(npcName, "Why are you still here? The garden is outside.");
                break;

            default:
                dialogueManager.ShowDialogue(npcName, "I am busy with paperwork. Do not make me expel you.");
                break;
        }
    }
}