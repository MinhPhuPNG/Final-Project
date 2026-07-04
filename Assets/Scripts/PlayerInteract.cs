using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [Header("Resource Interaction")]
    public float resourceRange = 3f;

    [Header("UI")]
    public TextMeshProUGUI resourcePromptText;

    private InteractableResource currentResource;
    private bool isInteracting;

    private void Start()
    {
        if (resourcePromptText != null)
        {
            resourcePromptText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        FindNearbyResource();
        HandleKeyboardInteract();
    }

    private void FindNearbyResource()
    {
        Collider[] resourceHits = Physics.OverlapSphere(transform.position, resourceRange);
        InteractableResource closestResource = null;
        float closestResourceDistance = Mathf.Infinity;

        foreach (Collider hit in resourceHits)
        {
            InteractableResource resource = hit.GetComponentInParent<InteractableResource>();
            if (resource == null)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, resource.transform.position);
            if (distance < closestResourceDistance)
            {
                closestResourceDistance = distance;
                closestResource = resource;
            }
        }

        currentResource = closestResource;

        if (resourcePromptText != null)
        {
            if (currentResource != null)
            {
                resourcePromptText.text = "Press E to harvest";
                resourcePromptText.gameObject.SetActive(true);
            }
            else
            {
                resourcePromptText.gameObject.SetActive(false);
            }
        }
    }

    private void HandleKeyboardInteract()
    {
        if (isInteracting)
        {
            return;
        }

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryInteract();
        }
    }

    public void OnInteract(InputValue value)
    {
        if (value == null || !value.isPressed || isInteracting)
        {
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
    }

    private IEnumerator InteractRoutine()
    {
        isInteracting = true;

        if (resourcePromptText != null)
        {
            resourcePromptText.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(1f);

        if (currentResource != null)
        {
            currentResource.Interact();
        }

        yield return new WaitForSeconds(0.4f);
        isInteracting = false;
    }
}