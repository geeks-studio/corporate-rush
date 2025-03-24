using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Weapon Handlers")]
    [SerializeField] private MeleeWeaponHandler meleeWeaponHandler;
    [SerializeField] private RangedWeaponHandler rangedWeaponHandler;
    [SerializeField] private Animator animator; // 🔥 Reference to Player Animator

    private void Awake()
    {
        if (meleeWeaponHandler == null)
            meleeWeaponHandler = GetComponent<MeleeWeaponHandler>();

        if (rangedWeaponHandler == null)
            rangedWeaponHandler = GetComponent<RangedWeaponHandler>();

        if (meleeWeaponHandler == null || rangedWeaponHandler == null)
            Debug.LogError("❌ Weapon handlers are missing on Player!");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left-click
        {
            if (meleeWeaponHandler.HasWeapon())
            {
                meleeWeaponHandler.PerformAttack();
            }
            else if (rangedWeaponHandler.HasWeapon())
            {
                rangedWeaponHandler.Shoot();
            }
        }

        if (Input.GetKeyDown(KeyCode.R)) // 🔄 Reload weapon
        {
            if (rangedWeaponHandler.HasWeapon())
            {
                rangedWeaponHandler.Reload();
            }
        }
    }

    public void EquipMeleeWeapon(GameObject weaponObject, GameObject weaponModel, AudioClip pickupSound)
    {
        if (meleeWeaponHandler == null) return;

        Debug.Log("Equipped melee weapon");

        Debug.Log(animator == null);

        // 🔥 Disable Ranged Animations when switching to Melee
        if (animator != null)
        {
            animator.SetBool("HoldingMeleeWeapon", true);
            animator.SetBool("HoldingRangedWeapon", false);
            Debug.Log("Check");
        }

        meleeWeaponHandler.EquipWeapon(weaponObject, weaponModel, pickupSound);
    }

    public void EquipRangedWeapon(GameObject weaponObject, GameObject weaponModel, AudioClip pickupSound, GameObject bullet, Transform shootPos)
    {
        if (rangedWeaponHandler == null) return;
        
        Debug.Log("Equipped ranged weapon");

        // 🔥 Disable Melee Animations when switching to Ranged
        if (animator != null)
        {
            animator.SetBool("HoldingMeleeWeapon", false);
            animator.SetBool("HoldingRangedWeapon", true);
        }

        rangedWeaponHandler.EquipWeapon(weaponObject, weaponModel, pickupSound, bullet, shootPos);
    }
}
