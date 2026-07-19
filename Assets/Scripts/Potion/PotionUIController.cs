using UnityEngine;

public class PotionUIController : MonoBehaviour
{
    [Header("UI Panel Reference")]
    public GameObject potionPanel;

    [Header("Specific Potion Images")]
    public GameObject potionImage2;
    public GameObject potionImage3;

    private QuestState lastState;

    void Start()
    {
        if (StoryManager.Instance != null)
        {
            lastState = StoryManager.Instance.currentQuestState;
            EvaluatePanelVisibility(lastState);
        }
    }

    void Update()
    {
        if (StoryManager.Instance != null && StoryManager.Instance.currentQuestState != lastState)
        {
            lastState = StoryManager.Instance.currentQuestState;
            EvaluatePanelVisibility(lastState);
        }
    }

    private void EvaluatePanelVisibility(QuestState state)
    {
        if (potionPanel == null) return;

        if (state >= QuestState.AlchemyTutorial && state != QuestState.GameComplete)
        {
            potionPanel.SetActive(true);
        }
        else
        {
            potionPanel.SetActive(false);
        }
    }

    public void RevealPotionImage(int id)
    {
        if (id == 2 && potionImage2 != null)
        {
            potionImage2.SetActive(true);
        }
        else if (id == 3 && potionImage3 != null)
        {
            potionImage3.SetActive(true);
        }
    }

    public bool HasPotion(int id)
    {
        if (id == 2 && potionImage2 != null) return potionImage2.activeSelf;
        if (id == 3 && potionImage3 != null) return potionImage3.activeSelf;
        return false;
    }

    // Check if BOTH required potions have been brewed
    public bool HasBothPotions()
    {
        return HasPotion(2) && HasPotion(3);
    }
}