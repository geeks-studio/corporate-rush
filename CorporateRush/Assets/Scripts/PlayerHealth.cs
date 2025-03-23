using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;

    [Header("Respawn & Checkpoint System")]
    [SerializeField] private float respawnDelay = 2f; // Time before respawn
    private Vector3 lastCheckpoint; // Stores the last checkpoint position
    private Rigidbody rb;
    private FirstPersonController playerController; // Reference to movement script

    [Header("UI References")]
    [SerializeField] private GameObject deathScreen; // Assign "You're Dead" UI

    void Start()
    {
        currentHealth = maxHealth;
        lastCheckpoint = transform.position; // Default spawn point
        rb = GetComponent<Rigidbody>();
        playerController = GetComponent<FirstPersonController>(); // Get movement script

        if (deathScreen != null)
        {
            deathScreen.SetActive(false); // Hide UI at start
        }
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

        // Disable player movement
        if (playerController != null) playerController.enabled = false;

        // Show death UI
        if (deathScreen != null)
        {
            deathScreen.SetActive(true);
        }

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

        // Hide death UI again
        if (deathScreen != null)
        {
            deathScreen.SetActive(false);
        }

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
