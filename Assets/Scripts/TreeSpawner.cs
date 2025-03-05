using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    public GameObject treePrefab; // The tree prefab to spawn
    public GameObject sampleArea; // The area to exclude
    public int numberOfTrees = 2; // Number of trees to spawn
    public float spawnRadius = 100f; // Radius around the origin to spawn trees
    public float bufferZone = 5f; // Extra distance outside the Sample area to avoid spawning too close

    private Bounds sampleBounds; // Bounding box of the Sample area

    void Start()
    {
        if (treePrefab == null)
        {
            Debug.LogError("Tree Prefab is not assigned in the Inspector!");
            return;
        }
        if (sampleArea == null)
        {
            Debug.LogError("Sample Area is not assigned in the Inspector!");
            return;
        }

        // Calculate the bounds of the Sample area
        CalculateSampleBounds();

        // Spawn the trees
        SpawnTrees();
    }

    void CalculateSampleBounds()
    {
        // Get all renderers in the Sample hierarchy to calculate the combined bounds
        Renderer[] renderers = sampleArea.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogError("No renderers found in Sample Area!");
            return;
        }

        // Initialize bounds with the first renderer
        sampleBounds = renderers[0].bounds;
        // Expand bounds to include all renderers
        foreach (Renderer renderer in renderers)
        {
            sampleBounds.Encapsulate(renderer.bounds);
        }

        // Expand the bounds by the buffer zone
        sampleBounds.Expand(bufferZone);
    }

    void SpawnTrees()
    {
        for (int i = 0; i < numberOfTrees; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            if (spawnPosition != Vector3.zero) // Check if a valid position was found
            {
                // Adjust the spawn position to the ground height using a raycast
                if (Physics.Raycast(spawnPosition + Vector3.up * 100f, Vector3.down, out RaycastHit hit, 200f))
                {
                    spawnPosition.y = hit.point.y;
                }

                // Spawn the tree
                GameObject tree = Instantiate(treePrefab, spawnPosition, Quaternion.identity);
                Debug.Log("Spawned tree at: " + spawnPosition);
            }
            else
            {
                Debug.LogWarning("Failed to find a valid spawn position for tree " + i);
                i--; // Retry this iteration
            }
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        int maxAttempts = 10; // Maximum attempts to find a valid position
        for (int i = 0; i < maxAttempts; i++)
        {
            // Generate a random position within the spawn radius
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = new Vector3(randomCircle.x, 0, randomCircle.y);

            // Check if the position is outside the Sample bounds
            if (!sampleBounds.Contains(spawnPosition))
            {
                return spawnPosition;
            }
        }

        // If no valid position is found after max attempts, return Vector3.zero
        return Vector3.zero;
    }
}
