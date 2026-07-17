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
        if (potionPanel != null) potionPanel.SetActive(false);
        if (potionImage2 != null) potionImage2.SetActive(false);
        if (potionImage3 != null) potionImage3.SetActive(false);

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
}