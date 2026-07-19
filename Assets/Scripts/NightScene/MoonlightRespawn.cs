using UnityEngine;

public class MoonlightRespawn : MonoBehaviour
{
    [Header("Respawn Settings")]
    public Transform gardenSpawnPoint;

    // Trigger continuously while standing in the light
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Moonlight"))
        {
            ResetPlayerState();
        }
    }

    private void ResetPlayerState()
    {
        Debug.LogWarning("[HAZARD] Caught by the moving moonlight! Resetting position and clearing inventory.");

        // 1. Clear resources
        ResourceCounter counter = FindFirstObjectByType<ResourceCounter>();
        if (counter != null)
        {
            counter.ConsumeResources(
                counter.GetMushroomCount(), 
                counter.GetPurpleFlowerCount(), 
                counter.GetTreeShroomCount()
            );
        }

        // 2. Teleport player
        if (gardenSpawnPoint != null)
        {
            CharacterController controller = GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;
            }

            transform.position = gardenSpawnPoint.position;

            if (controller != null)
            {
                controller.enabled = true;
            }
        }
        else
        {
            Debug.LogError("[HAZARD] Garden Spawn Point is unassigned.");
        }
    }
}