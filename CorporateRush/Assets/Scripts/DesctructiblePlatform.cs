using UnityEngine;
using System.Collections;

public class DestructiblePlatform : MonoBehaviour
{
    [Header("Destruction Settings")]
    [SerializeField] private float destructionDelay = 2f; // Time before the platform disappears
    [SerializeField] private GameObject destructionEffect; // Particle or animation effect
    [SerializeField] private GameObject debrisPrefab; // Optional debris that spawns

    private bool isTriggered = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            StartCoroutine(DestroyPlatform());
        }
    }

    private IEnumerator DestroyPlatform()
    {
        yield return new WaitForSeconds(destructionDelay);

        // 🔥 Play destruction effect if assigned
        if (destructionEffect != null)
        {
            Instantiate(destructionEffect, transform.position, Quaternion.identity);
        }

        // 🔥 Spawn debris if assigned
        if (debrisPrefab != null)
        {
            Instantiate(debrisPrefab, transform.position, transform.rotation);
        }

        // 🔥 Destroy the platform
        Destroy(gameObject);
    }
}