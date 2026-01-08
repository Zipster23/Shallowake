using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponModelInstantiation : MonoBehaviour
{

    public void UnloadWeapon()
    {
        gameObject.SetActive(false);
    }

    public void LoadWeapon()
    {
        gameObject.SetActive(true);
        /*
        weaponModel.transform.parent = transform;

        weaponModel.transform.localPosition = Vector3.zero;
        weaponModel.transform.localRotation = Quaternion.identity;
        weaponModel.transform.localScale = Vector3.one;
        */
    }

}
