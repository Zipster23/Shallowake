using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamageCollider : MonoBehaviour
{
    
    [Header("Damage Settings")]
    [SerializeField] private float damage = 25f;

    private List<GameObject> alreadyHitTargets = new List<GameObject>();

    private void OnEnable()
    {
        Debug.Log("WEAPON COLLIDER ENABLED!");
        alreadyHitTargets.Clear();
    }

    private void OnDisable()
    {
        Debug.Log("WEAPON COLLIDER DISABLED!");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Weapon hit something: {other.gameObject.name}");

        DamageableCharacter damageable = other.GetComponent<DamageableCharacter>();

        if(damageable != null)
        {
            if(!alreadyHitTargets.Contains(other.gameObject))
            {
                alreadyHitTargets.Add(other.gameObject);

                damageable.TakeDamage(damage);

                Debug.Log($"Hit {other.gameObject.name} for {damage} damage!");
            }
        }
        else
        {
            Debug.Log($"{other.gameObject.name} is not damageable!");
        }
    }



}
