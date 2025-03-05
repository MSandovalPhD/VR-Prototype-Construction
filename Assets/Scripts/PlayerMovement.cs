using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        enabled = true;
        Debug.Log("PlayerMovement enabled: " + enabled);

        // Initialize VR controllers if in VR mode
        if (UnityEngine.XR.XRSettings.enabled)
        {
            var rightHandDevices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller, rightHandDevices);
            if (rightHandDevices.Count > 0)
            {
                rightController = rightHandDevices[0];
            }

            var leftHandDevices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller, leftHandDevices);
            if (leftHandDevices.Count > 0)
            {
                leftController = leftHandDevices[0];
            }
        }
    }

    void FixedUpdate()
    {
        Debug.Log("FixedUpdate running. Time.timeScale: " + Time.timeScale);

        // Keep the excavator on the ground using a raycast
        if (Physics.Raycast(transform.position + Vector3.up * 1f, Vector3.down, out RaycastHit hit, 2f))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        }

        // VR Controls
        if (UnityEngine.XR.XRSettings.enabled)
        {
            // Right controller joystick for movement (forward/backward)
            if (rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rightJoystick))
            {
                if (rightJoystick.y > 0.2f) // Forward
                {
                    Debug.Log("VR Right Joystick: Moving forward at speed: " + forwardSpeed);
                    Vector3 forwardVelocity = transform.forward * forwardSpeed;
                    rb.velocity = new Vector3(forwardVelocity.x, rb.velocity.y, forwardVelocity.z);
                }
                else if (rightJoystick.y < -0.2f) // Backward
                {
                    Debug.Log("VR Right Joystick: Moving backward at speed: " + -forwardSpeed);
                    Vector3 backwardVelocity = -transform.forward * forwardSpeed;
                    rb.velocity = new Vector3(backwardVelocity.x, rb.velocity.y, backwardVelocity.z);
                }
                else
                {
                    rb.velocity = new Vector3(0, rb.velocity.y, 0);
                }
            }

            // Left controller joystick for rotation (left/right)
            if (leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 leftJoystick))
            {
                if (leftJoystick.x < -0.2f) // Rotate left
                {
                    Debug.Log("VR Left Joystick: Rotating left.");
                    transform.Rotate(0, -rotationSpeed * Time.fixedDeltaTime, 0);
                }
                else if (leftJoystick.x > 0.2f) // Rotate right
                {
                    Debug.Log("VR Left Joystick: Rotating right.");
                    transform.Rotate(0, rotationSpeed * Time.fixedDeltaTime, 0);
                }
            }
        }
        else // Non-VR Controls
        {
            // Swerve left with A key (rotate counterclockwise around Y axis)
            if (Input.GetKey(KeyCode.A))
            {
                Debug.Log("A key pressed. Rotating left.");
                transform.Rotate(0, -rotationSpeed * Time.fixedDeltaTime, 0);
            }
            // Swerve right with B key (rotate clockwise around Y axis)
            else if (Input.GetKey(KeyCode.B))
            {
                Debug.Log("B key pressed. Rotating right.");
                transform.Rotate(0, rotationSpeed * Time.fixedDeltaTime, 0);
            }

            // Move forward with X key (in the direction the excavator is facing)
            if (Input.GetKey(KeyCode.X))
            {
                Debug.Log("X key pressed. Moving forward at speed: " + forwardSpeed);
                Vector3 forwardVelocity = transform.forward * forwardSpeed;
                rb.velocity = new Vector3(forwardVelocity.x, rb.velocity.y, forwardVelocity.z);
            }
            // Move backward with Y key (in the opposite direction the excavator is facing)
            else if (Input.GetKey(KeyCode.Y))
            {
                Debug.Log("Y key pressed. Moving backward at speed: " + -forwardSpeed);
                Vector3 backwardVelocity = -transform.forward * forwardSpeed;
                rb.velocity = new Vector3(backwardVelocity.x, rb.velocity.y, backwardVelocity.z);
            }
            // Stop horizontal movement when neither X nor Y is pressed
            else
            {
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
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
}