using UnityEngine;
using System.Collections;

public class HingeToggleDoor : InteractableResource
{
    [Header("Automatic Close Settings")]
    [SerializeField] private float waitTimeBeforeClose = 3f;
    [SerializeField] private float doorSpeed = 150f;

    private bool isRunningCycle = false;

    private void Awake()
    {
        usesRemaining = 999999;
        destroyWhenEmpty = false;
    }

    public override void Interact()
    {
        // Only allow the door to be triggered if it isn't already moving
        if (!isRunningCycle)
        {
            StartCoroutine(DoorOpenCloseCycle());
        }
    }

    private IEnumerator DoorOpenCloseCycle()
    {
        isRunningCycle = true;

        HingeJoint hinge = GetComponent<HingeJoint>();
        if (hinge != null)
        {
            hinge.useMotor = true;
            JointMotor doorMotor = hinge.motor;

            // 1. Swing Open
            doorMotor.targetVelocity = doorSpeed; // Change to -doorSpeed if it swings backward
            hinge.motor = doorMotor;

            // 2. Wait while the player walks through
            yield return new WaitForSeconds(waitTimeBeforeClose);

            // 3. Swing Closed (Reversing the velocity)
            doorMotor.targetVelocity = -doorSpeed; // Change to doorSpeed if it swings backward
            hinge.motor = doorMotor;

            // 4. Wait for it to fully shut, then turn off the motor so it stays closed
            yield return new WaitForSeconds(1.5f);
            doorMotor.targetVelocity = 0f;
            hinge.motor = doorMotor;
        }

        isRunningCycle = false;
    }
}