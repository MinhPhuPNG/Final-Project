using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI Panel References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueContentText;
    public float textSpeed = 0.03f;

    private string[] dialogueLines;
    private int lineIndex;
    private Coroutine typeCoroutine;

    public bool IsDialogueActive { get; private set; }

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

    public static DialogueManager EnsureInstance()
    {
        if (Instance != null)
        {
            return Instance;
        }

        Instance = FindFirstObjectByType<DialogueManager>();
        if (Instance != null)
        {
            return Instance; // Fixed: Safely return the scene instance if found
        }

        GameObject managerObject = new GameObject("DialogueManager");
        Instance = managerObject.AddComponent<DialogueManager>();
        return Instance;
    }

    private void Update()
    {
        if (!IsDialogueActive)
        {
            return;
        }

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ContinueDialogue();
        }
    }

    public void ShowDialogue(string speakerName, string dialogueText)
    {
        ShowDialogue(speakerName, new[] { dialogueText });
    }

    public void ShowDialogue(string speakerName, string[] lines)
    {
        if (dialoguePanel == null || speakerNameText == null || dialogueContentText == null || lines == null || lines.Length == 0)
        {
            return;
        }

        dialogueLines = lines;
        lineIndex = 0;
        dialoguePanel.SetActive(true);
        speakerNameText.text = speakerName;
        IsDialogueActive = true;
        StartTypingCurrentLine();
    }

    public void HideDialogue()
    {
        if (typeCoroutine != null)
        {
            StopCoroutine(typeCoroutine);
            typeCoroutine = null;
        }

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        dialogueContentText.text = string.Empty;
        dialogueLines = null;
        lineIndex = 0;
        IsDialogueActive = false;
    }

    private void StartTypingCurrentLine()
    {
        if (typeCoroutine != null)
        {
            StopCoroutine(typeCoroutine);
        }

        dialogueContentText.text = string.Empty;
        typeCoroutine = StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine()
    {
        string currentLine = dialogueLines[lineIndex];

        foreach (char c in currentLine.ToCharArray())
        {
            dialogueContentText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        typeCoroutine = null;
    }

    private void ContinueDialogue()
    {
        if (dialogueLines == null || dialogueLines.Length == 0)
        {
            return;
        }

        string currentLine = dialogueLines[lineIndex];
        
        // If the text is still typing out, instantly complete it instead of moving forward
        if (dialogueContentText.text != currentLine)
        {
            if (typeCoroutine != null)
            {
                StopCoroutine(typeCoroutine);
                typeCoroutine = null;
            }

            dialogueContentText.text = currentLine;
            return;
        }

        // If the line was already complete, move to the next line or close out
        if (lineIndex < dialogueLines.Length - 1)
        {
            lineIndex++;
            StartTypingCurrentLine();
        }
        else
        {
            HideDialogue();
        }
    }
}