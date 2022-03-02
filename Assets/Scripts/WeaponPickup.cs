using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] RaycastWeapon weaponPrefab;

    private void OnTriggerEnter(Collider other)
    {
        ActiveWeapon activeWeapon = other.gameObject.GetComponent<ActiveWeapon>();

        if (activeWeapon)
        {
            RaycastWeapon newWeapon = Instantiate(weaponPrefab);

            activeWeapon.EquipWeapon(newWeapon);
        }
    }
}
