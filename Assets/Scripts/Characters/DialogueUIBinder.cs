using UnityEngine;
using TMPro;

public class DialogueUIBinder : MonoBehaviour
{
    [Header("Drag Scene Dialogue UI Here")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueContentText;

    private void Start()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.SetUIReferences(dialoguePanel, speakerNameText, dialogueContentText);
        }
    }
}