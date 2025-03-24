using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] private GameObject weaponModel;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private bool isRangedWeapon;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootPoint;

    private void OnTriggerEnter(Collider other)
    {
        PlayerWeapon playerWeapon = other.GetComponent<PlayerWeapon>();
        if (playerWeapon == null)
        {
            Debug.LogError("❌ PlayerWeapon script is missing on Player!");
            return;
        }

        if (isRangedWeapon)
        {
            playerWeapon.EquipRangedWeapon(gameObject, weaponModel, pickupSound, bulletPrefab, shootPoint);
        }
        else
        {
            playerWeapon.EquipMeleeWeapon(gameObject, weaponModel, pickupSound);
        }
    }
}