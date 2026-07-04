using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI Panel References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueContentText;

    public bool IsDialogueActive { get; private set; }

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
        }
    }

    public void ShowDialogue(string speakerName, string dialogueText)
    {
        if (dialoguePanel == null || speakerNameText == null || dialogueContentText == null)
        {
            return;
        }

        dialoguePanel.SetActive(true);
        IsDialogueActive = true;
        speakerNameText.text = speakerName;
        dialogueContentText.text = dialogueText;
    }

    public void HideDialogue()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        IsDialogueActive = false;
    }

}