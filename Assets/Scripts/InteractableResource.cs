using UnityEngine;

public class InteractableResource : MonoBehaviour
{
    public string resourceName = "Mushroom";
    public int amountPerHarvest = 1;
    public int usesRemaining = 3;

    public string promptText = "Press E to harvest";
    public bool destroyWhenEmpty = true;

    private ResourceCounter resourceCounter;

    void Start()
    {
        resourceCounter = FindFirstObjectByType<ResourceCounter>();
    }

    public virtual void Interact()
    {
        if (usesRemaining <= 0)
        {
            return;
        }
        if (resourceCounter != null)
        {
            resourceCounter.AddResource(resourceName, amountPerHarvest);
        }
        usesRemaining--;
        if (usesRemaining <= 0 && destroyWhenEmpty)
        {
            gameObject.SetActive(false);
        }
    }
}
