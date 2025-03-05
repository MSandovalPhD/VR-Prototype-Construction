using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    public float forwardSpeed = 2f;    // Speed for forward/backward movement (meters per second)
    public float rotationSpeed = 100f; // Speed of rotation/swerving (degrees per second)

    // Ground bounds (adjust based on your ground's size and position)
    private float groundMinX = -50f;   // Ground extends from X: -50 to 50
    private float groundMaxX = 50f;
    private float groundMinZ = -50f;   // Ground extends from Z: -50 to 50
    private float groundMaxZ = 50f;

    // VR input devices
    private InputDevice rightController;
    private InputDevice leftController;
    private bool controllersInitialized = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        enabled = true;
        Debug.Log("PlayerMovement enabled: " + enabled);

        // Disable Rigidbody gravity (we'll handle grounding manually)
        //rb.useGravity = false;

        // Initial ground alignment
        //AlignToGround();

        // Initialize VR controllers
        InitializeControllers();
    }

    void Update()
    {
        // Reattempt controller initialization if not yet initialized
        if (!controllersInitialized)
        {
            InitializeControllers();
        }
    }

    void FixedUpdate()
    {
        Debug.Log("FixedUpdate running. Time.timeScale: " + Time.timeScale);

        // Keep the excavator on the ground using a raycast
        //AlignToGround();

        // VR Controls
        if (XRGeneralSettings.Instance.Manager.isInitializationComplete)
        {
            // Right controller: A (forward), B (backward)
            if (rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool aButton))
            {
                Debug.Log("A Button State: " + aButton);
                if (aButton)
                {
                    Debug.Log("VR Right Controller A Button: Moving forward at speed: " + forwardSpeed);
                    Vector3 forwardVelocity = transform.forward * forwardSpeed;
                    rb.velocity = new Vector3(forwardVelocity.x, 0, forwardVelocity.z);
                }
            }
            if (rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool bButton))
            {
                Debug.Log("B Button State: " + bButton);
                if (bButton)
                {
                    Debug.Log("VR Right Controller B Button: Moving backward at speed: " + -forwardSpeed);
                    Vector3 backwardVelocity = -transform.forward * forwardSpeed;
                    rb.velocity = new Vector3(backwardVelocity.x, 0, backwardVelocity.z);
                }
            }
        
            // Left controller: X (rotate left), Y (rotate right)
            if (leftController.TryGetFeatureValue(CommonUsages.primaryButton, out bool xButton))
            {
                Debug.Log("X Button State: " + xButton);
                if (xButton)
                {
                    Debug.Log("VR Left Controller X Button: Rotating left.");
                    transform.Rotate(0, -rotationSpeed * Time.fixedDeltaTime, 0);
                }
            }
            if (leftController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool yButton))
            {
                Debug.Log("Y Button State: " + yButton);
                if (yButton)
                {
                    Debug.Log("VR Left Controller Y Button: Rotating right.");
                    transform.Rotate(0, rotationSpeed * Time.fixedDeltaTime, 0);
                }
            }
        }

        // Check if the player is outside the ground bounds or falls below Y = -1
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
}