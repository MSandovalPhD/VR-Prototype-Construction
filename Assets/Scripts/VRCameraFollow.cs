using UnityEngine;

public class VRCameraFollow : MonoBehaviour
{
    public Transform target; // The BAM_Large_Excavator to follow
    public Vector3 offset = new Vector3(0, 5f, -5f); // Position above and behind the excavator

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.position + target.TransformDirection(offset);
            transform.position = targetPosition;
        }
    }
}