using UnityEngine;
using System.Collections;

public class Elevator : MonoBehaviour
{
    [Header("Elevator Settings")]
    [SerializeField] private Transform startPoint;  // Elevator start position
    [SerializeField] private Transform endPoint;    // Elevator end position
    [SerializeField] private float travelTime = 3f; // Time to reach the top
    private bool isMoving = false;
    private Vector3 targetPosition;
    
    private void Start()
    {
        // Ensure elevator starts at startPoint
        transform.position = startPoint.position;
        targetPosition = endPoint.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isMoving)
        {
            StartCoroutine(MoveElevator(collision.gameObject));
        }
    }

    private IEnumerator MoveElevator(GameObject player)
    {
        isMoving = true;
        float elapsedTime = 0f;
        Vector3 initialPosition = transform.position;
        
        // Attach player to the elevator
        player.transform.parent = transform;

        while (elapsedTime < travelTime)
        {
            transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / travelTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition; // Ensure final position
        player.transform.parent = null; // Detach player after reaching the top
        isMoving = false;
    }
}