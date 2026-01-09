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
        gameObject.transform.parent = transform;

        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localRotation = Quaternion.identity;
        gameObject.transform.localScale = Vector3.one;
    }

}
