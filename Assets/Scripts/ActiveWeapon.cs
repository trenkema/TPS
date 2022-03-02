using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;
using UnityEditor.Animations;

public enum WeaponSlot { Primary = 0, Secondary = 1 }

public class ActiveWeapon : MonoBehaviour
{
    [SerializeField] Transform crossHairTarget;

    [SerializeField] CharacterController playerColliderToIgnore;

    [SerializeField] Rig handIK;

    [SerializeField] Animator rigController;

    [SerializeField] Transform[] weaponSlots;

    RaycastWeapon[] equippedWeapons = new RaycastWeapon[2];

    int activeWeaponIndex = 0;

    bool isHolstered = false;

    private void Start()
    {
        rigController.updateMode = AnimatorUpdateMode.AnimatePhysics;
        rigController.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
        rigController.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        rigController.updateMode = AnimatorUpdateMode.Normal;
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            var weapon = GetWeapon(activeWeaponIndex);

            if (weapon && !isHolstered)
                weapon.StartFiring();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            var weapon = GetWeapon(activeWeaponIndex);

            if (weapon)
                weapon.StopFiring();
        }
    }

    // Equipping
    public void OnHolster(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            ToggleActivateWeapon();
        }
    }

    public void OnEquipPrimary(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            SetActivateWeapon(WeaponSlot.Primary);
        }
    }

    public void OnEquipSecondary(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            SetActivateWeapon(WeaponSlot.Secondary);
        }
    }

    public void EquipWeapon(RaycastWeapon newWeapon)
    {
        int weaponSlotIndex = (int)newWeapon.weaponSlot;
        var weapon = GetWeapon(weaponSlotIndex);

        if (weapon)
        {
            Destroy(weapon.gameObject);
        }

        weapon = newWeapon;

        weapon.raycastDestination = crossHairTarget;
        weapon.transform.SetParent(weaponSlots[weaponSlotIndex], false);

        equippedWeapons[weaponSlotIndex] = weapon;

        SetActivateWeapon(newWeapon.weaponSlot);
    }

    // Activating & Switching
    private void ToggleActivateWeapon()
    {
        bool isHolstered = rigController.GetBool("holster_weapon");

        if (isHolstered)
        {
            StartCoroutine(ActivateWeapon(activeWeaponIndex));
        }
        else
        {
            StartCoroutine(HolsterWeapon(activeWeaponIndex));
        }
    }

    private void SetActivateWeapon(WeaponSlot weaponSlot)
    {
        int holsterIndex = activeWeaponIndex;
        int activateIndex = (int)weaponSlot;

        if (holsterIndex == activateIndex)
        {
            holsterIndex = -1;
        }

        StartCoroutine(SwitchWeapon(holsterIndex, activateIndex));
    }

    private IEnumerator SwitchWeapon(int holsterIndex, int activateIndex)
    {
        yield return StartCoroutine(HolsterWeapon(holsterIndex));
        yield return StartCoroutine(ActivateWeapon(activateIndex));
        activeWeaponIndex = activateIndex;
    }

    private IEnumerator HolsterWeapon(int index)
    {
        isHolstered = true;

        var weapon = GetWeapon(index);

        weapon.StopFiring();

        if (weapon)
        {
            rigController.SetBool("holster_weapon", true);

            do
            {
                yield return new WaitForEndOfFrame();
            }
            while (rigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f);
        }
    }

    private IEnumerator ActivateWeapon(int index)
    {
        var weapon = GetWeapon(index);

        if (weapon)
        {
            rigController.SetBool("holster_weapon", false);
            rigController.Play("equip_" + weapon.weaponName);

            do
            {
                yield return new WaitForEndOfFrame();
            }
            while (rigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f);

            isHolstered = false;
        }
    }

    private RaycastWeapon GetWeapon(int index)
    {
        if (index < 0 || index >= equippedWeapons.Length)
            return null;

        return equippedWeapons[index];
    }
}
