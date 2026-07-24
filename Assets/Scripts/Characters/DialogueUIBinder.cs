using UnityEngine;
using TMPro;

public class DialogueUIBinder : MonoBehaviour
{
    [Header("Drag Scene Dialogue UI Here")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueContentText;

    private void Awake()
    {
        DialogueManager.EnsureInstance().RegisterSceneUI(dialoguePanel, speakerNameText, dialogueContentText);
    }
}