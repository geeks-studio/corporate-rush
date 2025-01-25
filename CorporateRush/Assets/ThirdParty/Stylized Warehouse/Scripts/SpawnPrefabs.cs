using UnityEngine;

namespace ShelfGenerator
{
    public class SpawnPrefabs : MonoBehaviour
    {
        public GameObject prefabToSpawn; // Prefab to spawn
        public int numberOfPrefabs = 5; // Number of prefabs to spawn
        public int numberOfFloors = 3; // Number of floors for each prefab
        public float prefabSpawnDistance = 1f; // Distance between each spawned prefab
        public float floorSpawnDistance = 0.5f; // Distance between each spawned floor

        private void Start()
        {
            SpawnPrefabsInDirection();
        }

        private void SpawnPrefabsInDirection()
        {
            Vector3 spawnDirection = transform.right; // Spawn direction along the local X-axis of the game object
            Quaternion spawnRotation = transform.rotation; // Use the rotation of the game object for spawning

            for (int i = 0; i < numberOfPrefabs; i++)
            {
                Vector3 prefabSpawnPosition = transform.position + spawnDirection * prefabSpawnDistance * i;

                for (int j = 0; j < numberOfFloors; j++)
                {
                    Vector3 floorSpawnPosition = prefabSpawnPosition + transform.up * floorSpawnDistance * j;

                    // Instantiate the prefab at the calculated position and rotation
                    GameObject spawnedPrefab = Instantiate(prefabToSpawn, floorSpawnPosition, spawnRotation);

                    // Parent the spawned prefab to this GameObject for organizational purposes
                    spawnedPrefab.transform.parent = transform;
                }
            }
        }
    }
}
