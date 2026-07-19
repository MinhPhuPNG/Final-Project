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
                objectiveText.text = "Objective: Exit the building and find the Gardening Club.";
                break;
            case QuestState.GardeningClubTutorial:
                objectiveText.text = "Objective: Speak with the gardener and do as she asks.";
                break;
            case QuestState.AlchemyDelivery:
                objectiveText.text = "Objective: Bring the plants to the Alchemy classroom.";
                break;
            case QuestState.AlchemyTutorial:
                objectiveText.text = "Objective: Talk to the Alchemist";
                break;
            case QuestState.MeetOccult:
                objectiveText.text = "Objective: Enter the Graveyard to find your last chance.";
                break;
            case QuestState.NightShift:
                objectiveText.text = "Objective: Ask for your first task.";
                break;
            case QuestState.GardenHarvest:
                objectiveText.text = "Objective: Harvest ingredients, avoid the moonlight. Return to her when complete";
                break;
            case QuestState.PotionBrew:
                objectiveText.text = "Objective: Brew in remaining cauldrons.";
                break;
            case QuestState.PotionsDone:
                objectiveText.text = "Objective: Return to her for the last task.";
                break;
            case QuestState.ConsultOccult:
                objectiveText.text = "Objective: Find the spellbook.";
                break;
            case QuestState.FinalSummoning:
                objectiveText.text = "Objective: Complete the ritual";
                break;
            case QuestState.GameComplete:
                objectiveText.text = "Objective: Congratulations! You finished initiation.";
                break;
            default:
                objectiveText.text = "Objective: Explore and look for clues.";
                break;
        }
    }
}