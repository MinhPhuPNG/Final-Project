using System;
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
    private Action onDialogueComplete;

    public bool IsDialogueActive { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public static DialogueManager EnsureInstance()
    {
        if (Instance == null)
        {
            Instance = FindFirstObjectByType<DialogueManager>();
        }

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
        ShowDialogue(speakerName, new[] { dialogueText }, null);
    }

    public void ShowDialogue(string speakerName, string dialogueText, Action onComplete)
    {
        ShowDialogue(speakerName, new[] { dialogueText }, onComplete);
    }

    public void ShowDialogue(string speakerName, string[] lines)
    {
        ShowDialogue(speakerName, lines, null);
    }

    public void ShowDialogue(string speakerName, string[] lines, Action onComplete)
    {
        if (dialoguePanel == null || speakerNameText == null || dialogueContentText == null || lines == null || lines.Length == 0)
        {
            onComplete?.Invoke();
            return;
        }

        dialogueLines = lines;
        lineIndex = 0;
        onDialogueComplete = onComplete;

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

        if (dialogueContentText != null)
        {
            dialogueContentText.text = string.Empty;
        }

        dialogueLines = null;
        lineIndex = 0;
        IsDialogueActive = false;

        Action callback = onDialogueComplete;
        onDialogueComplete = null;
        callback?.Invoke();
    }

    private void StartTypingCurrentLine()
    {
        if (typeCoroutine != null)
        {
            StopCoroutine(typeCoroutine);
        }

        if (dialogueContentText != null)
        {
            dialogueContentText.text = string.Empty;
        }

        typeCoroutine = StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine()
    {
        if (dialogueLines == null || dialogueLines.Length == 0 || dialogueContentText == null)
        {
            typeCoroutine = null;
            yield break;
        }

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

        if (dialogueContentText == null)
        {
            HideDialogue();
            return;
        }

        string currentLine = dialogueLines[lineIndex];

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