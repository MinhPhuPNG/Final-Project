using UnityEngine;

public class CounselorNPC : MonoBehaviour, ITalkable
{
    public string npcName = "Counselor";
    private bool hasTalkedFirstTime = false;

    public void Talk()
    {
        StoryManager storyManager = StoryManager.EnsureInstance();
        DialogueManager dialogueManager = DialogueManager.Instance;

        if (dialogueManager == null)
        {
            return;
        }

        if (!hasTalkedFirstTime && storyManager.currentQuestState == QuestState.TalkToCounselor)
        {
            dialogueManager.ShowDialogue(npcName, "Listen to me carefully. You must join a student club immediately, you've already been rejected from all the others, I know it's hard but you risk expulsion. The gardening club is outside if you'd like to tryout.");
            hasTalkedFirstTime = true;
            storyManager.AdvanceQuest(QuestState.GoToGardeningClub);
            return;
        }

        if (storyManager.currentQuestState == QuestState.GoToGardeningClub)
        {
            dialogueManager.ShowDialogue(npcName, "Why are you still here? The garden is outside.");
        }
        else
        {
            dialogueManager.ShowDialogue(npcName, "I am busy with paperwork. Do not make me expel you.");
        }
    }
}