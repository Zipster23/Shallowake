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
        alreadyHitTargets.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
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
    }

    

}
