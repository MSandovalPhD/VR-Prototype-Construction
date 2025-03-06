using UnityEngine;

public class VRCameraFollow : MonoBehaviour
{
    public Transform target; // The BAM_Large_Excavator to follow
    public Vector3 offset = new Vector3(0, 1.5f, -5f); // Offset to position the VR camera above the excavator
    public bool alignYawWithTarget = true;

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.position + offset;
            transform.position = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z);

            if (alignYawWithTarget)
            {
                // Only set the Y rotation (yaw) to match the excavator, preserving head tracking for X and Z
                Quaternion targetRotation = Quaternion.Euler(0, target.rotation.eulerAngles.y, 0);
                transform.rotation = targetRotation;
            }
        }
    }
}