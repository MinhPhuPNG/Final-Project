using UnityEngine;

public enum QuestState
{
    TalkToCounselor,
    GoToGardeningClub,
    GardeningClubTutorial,
    AlchemyDelivery,
    AlchemyTutorial,
    MeetOccult,
    GardenHarvest,
    PotionBrew,
    ConsultOccult,
    FinalSummoning,
    GameComplete
}

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance { get; private set; }

    [Header("Current Story Step")]
    public QuestState currentQuestState = QuestState.TalkToCounselor;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void AdvanceQuest(QuestState nextState)
    {
        currentQuestState = nextState;
        Debug.Log($"[STORY UPDATE]: Objective is now {currentQuestState}");
    }
}