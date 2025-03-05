using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private PlayerMovement movement;
    private Renderer renderer; // For visual feedback

    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        renderer = GetComponentInChildren<Renderer>();

        if (movement == null)
        {
            Debug.LogError("PlayerMovement component not found on " + gameObject.name);
        }
        if (renderer == null)
        {
            Debug.LogError("Renderer component not found on " + gameObject.name);
        }
    }

    private void OnCollisionEnter(Collision collisionInfo)
    {
        Debug.Log("Collision detected with: " + collisionInfo.collider.name + ", Tag: " + collisionInfo.collider.tag);
        if (collisionInfo.collider.CompareTag("Obstacle"))
        {
            Debug.Log("Collided with Obstacle! Ending game.");
            if (renderer != null)
            {
                renderer.material.color = Color.red; // Visual feedback
            }
            movement.enabled = false;
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
        else
        {
            Debug.Log("Collided object does not have tag 'Obstacle'. Tag found: " + collisionInfo.collider.tag);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger detected with: " + other.name + ", Tag: " + other.tag);
        if (other.CompareTag("WinArea"))
        {
            Vector3 excavatorPos = transform.position;
            Vector3 storagePlacePos = other.transform.position;
            float distanceToCenter = Vector3.Distance(new Vector3(excavatorPos.x, 0, excavatorPos.z), new Vector3(storagePlacePos.x, 0, storagePlacePos.z));
            Debug.Log($"Distance to center of WinArea: {distanceToCenter}");

            if (distanceToCenter > 10f) // Increased from 1f to 2f
            {
                Debug.Log("Excavator is in the middle of the WinArea! You win!");
                if (renderer != null)
                {
                    renderer.material.color = Color.green; // Visual feedback
                }
                movement.enabled = false;
                GameManager gameManager = FindAnyObjectByType<GameManager>();
                if (gameManager != null)
                {
                    gameManager.WinGame();
                }
                else
                {
                    Debug.LogError("GameManager not found! Cannot trigger win condition.");
                }
            }
        }
    }
}