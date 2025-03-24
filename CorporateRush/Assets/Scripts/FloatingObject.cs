using UnityEngine;
using DG.Tweening;

public class FloatingObject : MonoBehaviour
{
    [Header("Floating Settings")]
    [SerializeField] private float floatHeight = 0.5f; // How high it moves
    [SerializeField] private float floatDuration = 1.5f; // Time for one cycle

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 20f; // Degrees per second

    private void Start()
    {
        // 🔥 Start floating up and down
        transform.DOMoveY(transform.position.y + floatHeight, floatDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo); // Infinite loop up and down
    }

    private void Update()
    {
        // 🔥 Rotate continuously
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}