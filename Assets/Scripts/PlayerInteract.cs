using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interaction Ranges")]
    public float resourceRange = 3f;
    public float npcRange = 6f;

    [Header("UI")]
    public TextMeshProUGUI resourcePromptText;
    public TextMeshProUGUI npcPromptText;

    private InteractableResource currentResource;
    private ITalkable currentNPC;
    private bool isInteracting;

    private void Start()
    {
        if (resourcePromptText != null) resourcePromptText.gameObject.SetActive(false);
        if (npcPromptText != null) npcPromptText.gameObject.SetActive(false);
    }

    private void Update()
    {
        FindNearbyInteractable();
        HandleKeyboardInteract();
    }

    private void FindNearbyInteractable()
    {
        Collider[] resourceHits = Physics.OverlapSphere(transform.position, resourceRange);
        InteractableResource closestResource = null;
        float closestResourceDistance = Mathf.Infinity;

        foreach (Collider hit in resourceHits)
        {
            InteractableResource resource = hit.GetComponentInParent<InteractableResource>();
            if (resource == null) continue;

            float distance = Vector3.Distance(transform.position, resource.transform.position);
            if (distance < closestResourceDistance)
            {
                closestResourceDistance = distance;
                closestResource = resource;
            }
        }

        Collider[] npcHits = Physics.OverlapSphere(transform.position, npcRange);
        ITalkable closestNPC = null;
        float closestNPCDistance = Mathf.Infinity;

        foreach (Collider hit in npcHits)
        {
            ITalkable npc = GetTalkableFromCollider(hit);
            if (npc == null) continue;

            float distance = Vector3.Distance(transform.position, hit.transform.position);
            if (distance < closestNPCDistance)
            {
                closestNPCDistance = distance;
                closestNPC = npc;
            }
        }

        currentResource = closestResource;
        currentNPC = closestNPC;

        if (resourcePromptText != null)
        {
            resourcePromptText.gameObject.SetActive(currentResource != null);
            if (currentResource != null)
            {
                resourcePromptText.text = "Press E to harvest";
            }
        }

        if (npcPromptText != null)
        {
            npcPromptText.gameObject.SetActive(currentNPC != null && currentResource == null);
            if (currentNPC != null && currentResource == null)
            {
                npcPromptText.text = "Press E to talk";
            }
        }
    }

    private void HandleKeyboardInteract()
    {
        if (isInteracting) return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            DialogueManager dialogueManager = DialogueManager.Instance;
            if (dialogueManager != null && dialogueManager.IsDialogueActive)
            {
                dialogueManager.HideDialogue();
                return;
            }

            TryInteract();
        }
    }

    public void OnInteract(InputValue value)
    {
        if (value == null || !value.isPressed || isInteracting) return;

        DialogueManager dialogueManager = DialogueManager.Instance;
        if (dialogueManager != null && dialogueManager.IsDialogueActive)
        {
            dialogueManager.HideDialogue();
            return;
        }

        TryInteract();
    }

    private void TryInteract()
    {
        if (currentResource != null)
        {
            StartCoroutine(InteractRoutine());
        }
        else if (currentNPC != null)
        {
            currentNPC.Talk();
        }
    }

    private IEnumerator InteractRoutine()
    {
        isInteracting = true;
        if (resourcePromptText != null) resourcePromptText.gameObject.SetActive(false);
        if (npcPromptText != null) npcPromptText.gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);

        if (currentResource != null)
        {
            currentResource.Interact();
        }

        yield return new WaitForSeconds(0.4f);
        isInteracting = false;
    }

    private ITalkable GetTalkableFromCollider(Collider hit)
    {
        Transform current = hit.transform;
        while (current != null)
        {
            foreach (MonoBehaviour component in current.GetComponents<MonoBehaviour>())
            {
                if (component is ITalkable talkable)
                {
                    return talkable;
                }
            }

            current = current.parent;
        }

        return null;
    }
}