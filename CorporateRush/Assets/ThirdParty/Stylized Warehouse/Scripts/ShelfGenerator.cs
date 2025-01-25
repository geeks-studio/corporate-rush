using UnityEngine;

namespace ShelfGenerator
{
    [System.Serializable]
    public class BoxSpawnData
    {
        public GameObject boxPrefab;
        [Range(0f, 1f)]
        public float spawnPercentage;
    }

    public class ShelfGenerator : MonoBehaviour
    {
        public BoxSpawnData[] boxSpawnData;
        public int numBoxesPerShelf;

        private void Start()
        {
            GenerateBoxes();
        }

        private void GenerateBoxes()
        {
            float boxColliderWidth = GetComponent<BoxCollider>().size.x;
            float boxColliderHeight = GetComponent<BoxCollider>().size.y;

            float boxWidth = boxColliderWidth / numBoxesPerShelf;
            float boxHeight = boxColliderHeight / 2f;

            Vector3 spawnDirection = transform.right;
            Vector3 rowStartPosition = transform.position - (numBoxesPerShelf - 1) * boxWidth * spawnDirection * 0.5f;

            for (int boxIndex = 0; boxIndex < numBoxesPerShelf; boxIndex++)
            {
                Vector3 boxPosition = rowStartPosition + boxIndex * boxWidth * spawnDirection;
                boxPosition.y += boxHeight;

                GameObject randomBoxPrefab = SelectRandomBoxPrefab();

                GameObject box = Instantiate(randomBoxPrefab, boxPosition, Quaternion.identity, transform);

                float randomRotationY = Random.Range(0, 4) * 90f;
                box.transform.rotation = Quaternion.Euler(0f, randomRotationY, 0f);
            }
        }

        private GameObject SelectRandomBoxPrefab()
        {
            float totalPercentage = 0f;
            foreach (BoxSpawnData data in boxSpawnData)
            {
                totalPercentage += data.spawnPercentage;
            }

            float randomValue = Random.value * totalPercentage;
            float cumulativePercentage = 0f;

            foreach (BoxSpawnData data in boxSpawnData)
            {
                cumulativePercentage += data.spawnPercentage;
                if (randomValue <= cumulativePercentage)
                {
                    return data.boxPrefab;
                }
            }

            return boxSpawnData[0].boxPrefab;
        }
    }
}
