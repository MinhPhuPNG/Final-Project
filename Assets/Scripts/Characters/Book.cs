using UnityEngine;

public class Book : InteractableNPC
{
    [Header("Book Content")]
    [TextArea(3, 5)]
    public string[] bookPages = new string[]
    {
        "Each step leaves a trace, without a scratch on the metal. You cannot go back",
        "A catalyst, a soul. Frequency without a box.",
        "--- 1: ARTIST'S VISION ---\n\nThey gave him a form, we must give him a soul.\n• 5 Purple Flowers\n• 2 Tree Shroom\n\nResult: Grants temporary bypass through dark miasma zones.",
        "--- 2: WOOD ---\n\nThe basis of his form\n• 5 Mushrooms\n• 4 Tree Shrooms\n\nThe Moonlight has touched all, for the sun lies dormant.",
        "The bubble has been popped"
    };

    private int currentPageIndex = 0;
    private bool IsReading = false;

    private void Start()
    {
        npcName = "Book"; 
    }

    public override void Talk()
    {
        DialogueManager dialogueManager = DialogueManager.EnsureInstance();
        if (dialogueManager == null) return;

        if (!IsReading)
        {
            IsReading = true;
            currentPageIndex = 0;
            DisplayPage(dialogueManager);
        }
        else
        {
            AdvancePage(dialogueManager);
        }
    }

    private void DisplayPage(DialogueManager dialogueManager)
    {
        if (currentPageIndex < bookPages.Length)
        {
            dialogueManager.ShowDialogue(npcName, bookPages[currentPageIndex]);
        }
    }

    private void AdvancePage(DialogueManager dialogueManager)
    {
        currentPageIndex++;

        if (currentPageIndex < bookPages.Length)
        {
            DisplayPage(dialogueManager);
        }
        else
        {
            dialogueManager.HideDialogue();
            IsReading = false;
        }
    }
}