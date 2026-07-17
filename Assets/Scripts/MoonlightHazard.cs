using UnityEngine;

public class MoonlightHazard : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Drag an empty GameObject located at your safe zone/respawn point here.")]
    public Transform safetySpawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RespawnPlayer(other.gameObject);
        }
    }

    private void RespawnPlayer(GameObject player)
    {
        CharacterController cc = player.GetComponent<CharacterController>();
        
        if (cc != null) cc.enabled = false;

        player.transform.position = safetySpawnPoint.position;

        if (cc != null) cc.enabled = true;

        Debug.Log("[HAZARD] Caught by moonlight! Teleported to safety.");
    }
}