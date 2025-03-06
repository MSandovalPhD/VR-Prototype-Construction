using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    public float forwardSpeed = 2f;    // Speed for forward/backward movement (meters per second)
    public float rotationSpeed = 100f; // Speed of rotation/swerving (degrees per second)
    public Transform vrCamera;
    public float headRotationSpeed = 50f;
    private float groundMinX = -50f;
    private float groundMaxX = 50f;
    private float groundMinZ = -50f;
    private float groundMaxZ = 50f;

    // VR input devices
    private InputDevice rightController;
    private InputDevice leftController;
    private bool controllersInitialized = false;
    private bool isBraking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        enabled = true;
        Debug.Log("PlayerMovement enabled: " + enabled);

        //rb.useGravity = false;

         //AlignToGround();

        InitializeControllers();
    }

    void Update()
    {
        if (!controllersInitialized)
        {
            InitializeControllers();
        }
    }

    void FixedUpdate()
    {
        Debug.Log("FixedUpdate running. Time.timeScale: " + Time.timeScale);

        // VR Controls
        if (XRGeneralSettings.Instance.Manager.isInitializationComplete)
        {
            if (rightController.isValid)
            {
                if (rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool aButton))
                {
                    Debug.Log("A Button State (Brake): " + aButton);
                    isBraking = aButton;
                }

                if (rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rightJoystick))
                {
                    Debug.Log("Right Joystick X: " + rightJoystick.x);
                    if (!isBraking)
                    {
                        if (rightJoystick.x < -0.2f) // Rotate left
                        {
                            Debug.Log("VR Right Joystick: Rotating left.");
                            transform.Rotate(0, -rotationSpeed * Time.fixedDeltaTime, 0);
                        }
                        else if (rightJoystick.x > 0.2f) // Rotate right
                        {
                            Debug.Log("VR Right Joystick: Rotating right.");
                            transform.Rotate(0, rotationSpeed * Time.fixedDeltaTime, 0);
                        }
                    }
                }
            }

            if (leftController.isValid)
            {
                if (leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 leftJoystick))
                {
                    Debug.Log("Left Joystick Y: " + leftJoystick.y);
                    if (!isBraking)
                    {
                        if (leftJoystick.y > 0.2f) // Forward
                        {
                            Debug.Log("VR Left Joystick: Moving forward at speed: " + forwardSpeed);
                            Vector3 forwardVelocity = transform.forward * forwardSpeed;
                            rb.velocity = new Vector3(forwardVelocity.x, rb.velocity.y, forwardVelocity.z);
                        }
                        else if (leftJoystick.y < -0.2f) // Backward
                        {
                            Debug.Log("VR Left Joystick: Moving backward at speed: " + -forwardSpeed);
                            Vector3 backwardVelocity = -transform.forward * forwardSpeed;
                            rb.velocity = new Vector3(backwardVelocity.x, rb.velocity.y, backwardVelocity.z);
                        }
                        else
                        {
                            rb.velocity = new Vector3(0, rb.velocity.y, 0);
                        }
                    }
                    else
                    {
                        rb.velocity = new Vector3(0, rb.velocity.y, 0);
                    }
                }
            }
                        
            if (vrCamera != null && !isBraking)
            {
                // Get the VR camera's Y rotation (head direction)
                float headYaw = vrCamera.eulerAngles.y;
                float excavatorYaw = transform.eulerAngles.y;
                float yawDifference = Mathf.DeltaAngle(excavatorYaw, headYaw);
                if (Mathf.Abs(yawDifference) > 5f) // Only rotate if the difference is more than 5 degrees
                {
                    float rotationAmount = (- 1) *  Mathf.Sign(yawDifference) * headRotationSpeed * Time.fixedDeltaTime;
                    transform.Rotate(0, rotationAmount, 0);
                    Debug.Log("Head tracking: Rotating excavator by " + rotationAmount + " degrees (Yaw Difference: " + yawDifference + ")");
                }
            }
        }

        Vector3 playerPos = rb.position;
        Debug.Log("Player position: " + playerPos);
        if (playerPos.y < -1 || playerPos.x < groundMinX || playerPos.x > groundMaxX || playerPos.z < groundMinZ || playerPos.z > groundMaxZ)
        {
            Debug.Log("Player is outside ground bounds or fell off! Ending game.");
            GameManager gameManager = FindAnyObjectByType<GameManager>();
            if (gameManager != null)
            {
                gameManager.EndGame();
            }
            else
            {
                Debug.LogError("GameManager not found! Cannot end game.");
            }
        }
    }

    void InitializeControllers()
    {
        var rightHandDevices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller, rightHandDevices);
        if (rightHandDevices.Count > 0)
        {
            rightController = rightHandDevices[0];
            Debug.Log("Right controller detected: " + rightController.name);
            controllersInitialized = true;
        }
        else
        {
            Debug.LogWarning("Right controller not detected!");
        }

        var leftHandDevices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller, leftHandDevices);
        if (leftHandDevices.Count > 0)
        {
            leftController = leftHandDevices[0];
            Debug.Log("Left controller detected: " + leftController.name);
            controllersInitialized = true;
        }
        else
        {
            Debug.LogWarning("Left controller not detected!");
        }
    }

    /*
    void AlignToGround()
    {
        // Cast a ray downward from a very high position to ensure it hits the ground
        Vector3 raycastOrigin = transform.position + Vector3.up * 1000f; // Start 1000 units above
        if (Physics.Raycast(raycastOrigin, Vector3.down, out RaycastHit hit, 2000f))
        {
            Vector3 newPosition = transform.position;
            newPosition.y = hit.point.y;
            transform.position = newPosition;
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            Debug.Log("Excavator aligned to ground at Y: " + newPosition.y);
        }
        else
        {
            Debug.LogWarning("Raycast did not hit the ground! Forcing excavator to Y: 0.");
            Vector3 newPosition = transform.position;
            newPosition.y = 0; // Force to Y: 0 if raycast fails
            transform.position = newPosition;
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }
    }
    */
}