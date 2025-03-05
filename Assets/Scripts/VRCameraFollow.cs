using UnityEngine;

public class VRCameraFollow : MonoBehaviour
{
    public Transform target; // The BAM_Large_Excavator to follow
    public Vector3 offset = new Vector3(0, 1.5f, 0); // Offset to position the VR camera above the excavator

    void LateUpdate()
    {
        if (target != null)
        {
            // Update the XR Rig's position to follow the excavator
            Vector3 targetPosition = target.position + offset;
            transform.position = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z);
        }
    }
}