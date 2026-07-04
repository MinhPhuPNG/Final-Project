using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerNpcInteraction : MonoBehaviour
{
    [Header("NPC Interaction")]
    public float npcRange = 6f;
    public TextMeshProUGUI npcPromptText;

    private CounselorNPC currentNPC;
    private DialogueManager dialogueManager;

    private void Start()
    {
        dialogueManager = DialogueManager.EnsureInstance();

        if (npcPromptText != null)
        {
            npcPromptText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        FindNearbyNpc();
    }

    private void FindNearbyNpc()
    {
        currentNPC = null;
        Collider[] hits = Physics.OverlapSphere(transform.position, npcRange);
        float closestDistance = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            CounselorNPC npc = hit.GetComponentInParent<CounselorNPC>();
            if (npc == null)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, hit.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                currentNPC = npc;
            }
        }

        if (npcPromptText != null)
        {
            bool showPrompt = currentNPC != null && (dialogueManager == null || !dialogueManager.IsDialogueActive);
            npcPromptText.gameObject.SetActive(showPrompt);

            if (showPrompt)
            {
                npcPromptText.text = "Press F to talk";
            }
        }
    }

    public void OnTalk(InputValue value)
    {
        if (value == null || !value.isPressed)
        {
            return;
        }

        if (dialogueManager != null && dialogueManager.IsDialogueActive)
        {
            dialogueManager.HideDialogue();
            return;
        }

        TryTalk();
    }

    private void TryTalk()
    {
        if (currentNPC != null)
        {
            currentNPC.Talk();
        }
    }
}
