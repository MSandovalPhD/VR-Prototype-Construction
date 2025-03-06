using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target; // The target to follow (BAM_Large_Excavator)
    public Vector3 offset = new Vector3(0, 5, -10); 
    public float smoothSpeed = 0.125f; // Speed of the smooth follow
    public float rotationSmoothSpeed = 5f; // Speed of the rotation smoothing
    public bool followRotation = true; // If true, camera rotates with the target
    public bool smoothLookAt = true; // If true, smoothly rotates to look at the target

    void Start()
    {
        if (UnityEngine.XR.XRSettings.enabled)
        {
            enabled = false;
        }
    }

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("FollowCamera target is not assigned!");
            return;
        }

        Vector3 desiredPosition;
        if (followRotation)
        {
            desiredPosition = target.position + target.TransformDirection(offset);
        }
        else
        {
            desiredPosition = target.position + offset;
        }

                Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Look at the target
        if (smoothLookAt)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmoothSpeed * Time.deltaTime);
        }
        else
        {
            transform.LookAt(target);
        }
    }
}