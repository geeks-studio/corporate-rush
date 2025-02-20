using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private Vector3 lastCheckpoint; // Stores the last checkpoint position

    void Start()
    {
        lastCheckpoint = transform.position; // Default spawn point
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CheckPoint"))
        {
            lastCheckpoint = other.transform.position; // Save checkpoint position
        }
        else if (other.CompareTag("DeathZone"))
        {
            Respawn(); // Respawn when hitting death zone
        }
    }

    private void Respawn()
    {
        transform.position = lastCheckpoint;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero; // Reset velocity to prevent weird physics glitches
        }
    }
}