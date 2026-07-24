using UnityEngine;

public enum QuestState
{
    TalkToCounselor,
    GoToGardeningClub,
    GardeningClubTutorial,
    AlchemyDelivery,
    AlchemyTutorial,
    MeetOccult,
    NightShift,
    GardenHarvest,
    PotionBrew,
    FindBook,
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
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    public void AdvanceQuest(QuestState nextState)
    {
        if (currentQuestState == nextState)
        {
            return;
        }

        currentQuestState = nextState;
        Debug.Log($"[STORY UPDATE]: Objective is now {currentQuestState}");
    }
}