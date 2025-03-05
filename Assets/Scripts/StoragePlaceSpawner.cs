using UnityEngine;

public class StoragePlaceSpawner : MonoBehaviour
{
    public GameObject storagePlacePrefab; // The StoragePlace (white plane) prefab to spawn
    public GameObject sampleArea; // The Sample object (with green hoardings)
    public float spawnRadius = 100f; // Radius around the origin to spawn the plane
    public float bufferZone = 5f; // Extra distance outside the Sample area to avoid spawning too close

    private Bounds sampleBounds; // Bounding box of the Sample area
    private GameObject spawnedStoragePlace; // Reference to the spawned StoragePlace

    void Start()
    {
        if (storagePlacePrefab == null)
        {
            Debug.LogError("StoragePlace Prefab is not assigned in the Inspector!");
            return;
        }
        if (sampleArea == null)
        {
            Debug.LogError("Sample Area is not assigned in the Inspector!");
            return;
        }

        // Calculate the bounds of the Sample area
        CalculateSampleBounds();

        // Spawn the StoragePlace
        SpawnStoragePlace();
    }

    void CalculateSampleBounds()
    {
        Renderer[] renderers = sampleArea.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogError("No renderers found in Sample Area!");
            return;
        }

        sampleBounds = renderers[0].bounds;
        foreach (Renderer renderer in renderers)
        {
            sampleBounds.Encapsulate(renderer.bounds);
        }

        sampleBounds.Expand(bufferZone);
    }

    void SpawnStoragePlace()
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        if (spawnPosition != Vector3.zero)
        {
            if (Physics.Raycast(spawnPosition + Vector3.up * 100f, Vector3.down, out RaycastHit hit, 200f))
            {
                spawnPosition.y = hit.point.y;
            }

            spawnedStoragePlace = Instantiate(storagePlacePrefab, spawnPosition, Quaternion.identity);
            Debug.Log("Spawned StoragePlace at: " + spawnPosition);

            BoxCollider collider = spawnedStoragePlace.GetComponent<BoxCollider>();
            if (collider == null)
            {
                collider = spawnedStoragePlace.AddComponent<BoxCollider>();
            }
            collider.isTrigger = true;
            // Set the collider size to match the plane (adjust based on your plane's dimensions)
            collider.size = new Vector3(10f, 0.1f, 10f); // Example: 4x4 plane with 0.1 height
            collider.center = new Vector3(0f, 0.05f, 0f); // Center slightly above the bottom
            spawnedStoragePlace.tag = "WinArea";
        }
        else
        {
            Debug.LogWarning("Failed to find a valid spawn position for StoragePlace.");
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        int maxAttempts = 10;
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = new Vector3(randomCircle.x, 0, randomCircle.y);

            if (!sampleBounds.Contains(spawnPosition))
            {
                Collider[] colliders = Physics.OverlapSphere(spawnPosition, 5f);
                bool overlap = false;
                foreach (Collider collider in colliders)
                {
                    if (collider.CompareTag("Obstacle") || collider.CompareTag("Player"))
                    {
                        overlap = true;
                        break;
                    }
                }
                if (!overlap)
                {
                    return spawnPosition;
                }
            }
        }
        return Vector3.zero;
    }

    public GameObject GetSpawnedStoragePlace()
    {
        return spawnedStoragePlace;
    }
}