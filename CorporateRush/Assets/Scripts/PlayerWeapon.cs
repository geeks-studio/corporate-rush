using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Weapon Handlers")]
    [SerializeField] private MeleeWeaponHandler meleeHandler;
    [SerializeField] private RangedWeaponHandler rangedHandler;
    [SerializeField] private Animator animator;

    private GameObject currentWeapon;
    private bool hasMelee = false;
    private bool hasRanged = false;
    private bool meleeEquipped = false;
    private bool rangedEquipped = false;

    private void Awake()
    {
        // Auto-assign if not linked in Inspector
        if (meleeHandler == null) meleeHandler = GetComponentInChildren<MeleeWeaponHandler>();
        if (rangedHandler == null) rangedHandler = GetComponentInChildren<RangedWeaponHandler>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && hasMelee)
        {
            EquipMelee();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && hasRanged)
        {
            EquipRanged();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (meleeEquipped)
                meleeHandler.PerformAttack();
            else if (rangedEquipped)
                rangedHandler.Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R) && rangedEquipped)
        {
            rangedHandler.Reload();
        }
    }

    public void EquipMeleeWeapon(GameObject pickupObject, GameObject weaponModel, AudioClip pickupSound)
    {
        meleeHandler.EquipWeapon(pickupObject, weaponModel, pickupSound);
        hasMelee = true;

        EquipMelee(); // Auto switch
    }

    public void EquipRangedWeapon(GameObject pickupObject, GameObject weaponModel, AudioClip pickupSound, GameObject bullet, Transform shootPoint)
    {
        rangedHandler.EquipWeapon(pickupObject, weaponModel, pickupSound, bullet, shootPoint);
        hasRanged = true;

        EquipRanged(); // Auto switch
    }

    private void EquipMelee()
    {
        meleeEquipped = true;
        rangedEquipped = false;

        meleeHandler.ShowWeapon(true);
        rangedHandler.ShowWeapon(false);

        if (animator != null)
        {
            animator.SetBool("HoldingMeleeWeapon", true);
            animator.SetBool("HoldingRangedWeapon", false);
        }
    }

    private void EquipRanged()
    {
        meleeEquipped = false;
        rangedEquipped = true;

        meleeHandler.ShowWeapon(false);
        rangedHandler.ShowWeapon(true);

        if (animator != null)
        {
            animator.SetBool("HoldingMeleeWeapon", false);
            animator.SetBool("HoldingRangedWeapon", true);
        }
    }
}
