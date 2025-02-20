using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float respawnDelay = 2f; // Time before respawn
    private int currentHealth;
    private Vector3 lastCheckpoint; // Stores the last checkpoint position
    private Rigidbody rb;
    private PlayerController playerController; // Reference to movement script

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        lastCheckpoint = transform.position; // Default spawn point
        rb = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerController>(); // Get movement script
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // Prevent taking damage when already dead

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            StartCoroutine(RespawnAfterDelay());
        }
    }

    public void KillPlayer()
    {
        if (isDead) return; // Prevent multiple respawn calls

        StartCoroutine(RespawnAfterDelay());
    }

    private IEnumerator RespawnAfterDelay()
    {
        isDead = true;
        if (playerController != null) playerController.enabled = false; // Disable movement

        // OPTIONAL: Add death animation, sound, or UI fade here
        yield return new WaitForSeconds(respawnDelay);

        // Respawn player at last checkpoint
        transform.position = lastCheckpoint;

        // Reset physics to prevent weird velocity issues
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Restore health
        currentHealth = maxHealth;

        // Re-enable player movement
        if (playerController != null) playerController.enabled = true;
        isDead = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CheckPoint"))
        {
            lastCheckpoint = other.transform.position; // Save checkpoint position
        }
        else if (other.CompareTag("DeathZone"))
        {
            KillPlayer(); // Respawn when hitting a death zone
        }
    }
}
