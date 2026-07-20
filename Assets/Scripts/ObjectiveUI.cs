using UnityEngine;
using TMPro;

public class ObjectiveUI : MonoBehaviour
{
    private TextMeshProUGUI objectiveText;
    private QuestState lastState;

    void Start()
    {
        objectiveText = GetComponent<TextMeshProUGUI>();
        if (StoryManager.Instance != null)
        {
            lastState = StoryManager.Instance.currentQuestState;
            UpdateObjectiveText(lastState);
        }
    }

    void Update()
    {
        if (StoryManager.Instance != null && StoryManager.Instance.currentQuestState != lastState)
        {
            lastState = StoryManager.Instance.currentQuestState;
            UpdateObjectiveText(lastState);
        }
    }

    private void UpdateObjectiveText(QuestState state)
    {
        switch (state)
        {
            case QuestState.TalkToCounselor:
                objectiveText.text = "Objective: Talk to the Counselor in her office.";
                break;
            case QuestState.GoToGardeningClub:
                objectiveText.text = "Objective: Exit the building to find the Garden.";
                break;
            case QuestState.GardeningClubTutorial:
                objectiveText.text = "Objective: Do as the Gardener asks.";
                break;
            case QuestState.AlchemyDelivery:
                objectiveText.text = "Objective: Bring the plants to the classroom.";
                break;
            case QuestState.AlchemyTutorial:
                objectiveText.text = "Objective: Try to brew a Potion and give it to the Alchemist.";
                break;
            case QuestState.MeetOccult:
                objectiveText.text = "Objective: Enter the Graveyard to find your last chance.";
                break;
            case QuestState.NightShift:
                objectiveText.text = "Objective: Ask for your first task.";
                break;
            case QuestState.GardenHarvest:
                objectiveText.text = "Objective: Harvest ingredients, then return to her. Avoid the moon's gaze.";
                break;
            case QuestState.PotionBrew:
                objectiveText.text = "Objective: Brew in remaining cauldrons, then return to her.";
                break;
            case QuestState.FindBook:
                objectiveText.text = "Objective: Find the spellbook.";
                break;
            case QuestState.FinalSummoning:
                objectiveText.text = "Complete the ritual";
                break;
            case QuestState.GameComplete:
                objectiveText.text = "Congratulations! You finished initiation. Welcome to the Club.";
                break;
            default:
                objectiveText.text = "Objective: Explore and look for clues.";
                break;
        }
    }
}