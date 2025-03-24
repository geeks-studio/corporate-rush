using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float speed = 50f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifetime = 5f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("❌ Bullet is missing Rigidbody!");
            return;
        }

        rb.linearVelocity = transform.forward * speed; // 🔥 Bullet moves immediately
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.useGravity = false; // 🔥 Disable gravity so bullets don’t fall
        Destroy(gameObject, lifetime); // Cleanup after some time
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"🚀 Bullet hit: {other.name}");
        
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Hitted enemy");
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject); // Bullet disappears on impact
        }
        else if (!other.CompareTag("Player")) // Prevent hitting player
        {
            //Destroy(gameObject); // Destroy if it hits a wall or floor
        }
    }
}