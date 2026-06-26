using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float headHeight = 0.5f; 

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 eyePosition = target.position + new Vector3(0, headHeight, 0);
            transform.position = eyePosition;
            
            transform.rotation = target.rotation;
        }
    }
}