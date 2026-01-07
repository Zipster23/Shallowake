using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponModelInstantiation : MonoBehaviour
{
    public GameObject weaponModel;

    public void UnloadWeapon()
    {
        if (weaponModel != null)
        {
            
        }
    }

    public void LoadWeapon()
    {
        weaponModel.transform.parent = transform;

        weaponModel.transform.localPosition = Vector3.zero;
        weaponModel.transform.localRotation = Quaternion.identity;
        weaponModel.transform.localScale = Vector3.one;
        
    }

}
