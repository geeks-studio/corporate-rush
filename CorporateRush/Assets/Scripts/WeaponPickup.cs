using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] private GameObject weaponModel; // Assign weapon mesh here
    [SerializeField] private AudioClip pickupSound; // Sound when picked up

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerWeapon>().EquipWeapon(gameObject, weaponModel, pickupSound);
        }
    }
}