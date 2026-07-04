using UnityEngine;

public enum QuestState
{
    TalkToCounselor,
    GoToGardeningClub,
    GardeningClubTutorial,
    AlchemyDelivery,
    AlchemyTutorial
    // GoToGraveyard,
    // CollectHerbs,
    // CreatePotion,
    // FindBook,
    // SummonSpirit
}

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance { get; private set; }

    [Header("Current Story Step")]
    public QuestState currentQuestState = QuestState.TalkToCounselor;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public static StoryManager EnsureInstance()
    {
        if (Instance != null)
        {
            return Instance;
        }

        GameObject managerObject = new GameObject("StoryManager");
        Instance = managerObject.AddComponent<StoryManager>();
        return Instance;
    }

    public void AdvanceQuest(QuestState nextState)
    {
        currentQuestState = nextState;
        Debug.Log($"[STORY UPDATE]: Objective is now {currentQuestState}");
    }
}