using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorManager : CharacterAnimatorManager
{
    PlayerManager player;

    [Header("Weapon Collider")]
    [SerializeField] private WeaponDamageCollider weaponDamageCollider;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    private void OnAnimatorMove()
    {
        if(player == null)
        {
            return;
        }

        if(player.applyRootMotion)
        {
            Vector3 velocity = player.animator.deltaPosition;

            player.characterController.Move(velocity);
            player.transform.rotation *= player.animator.deltaRotation;
        }
    }

    public void EnableDamageCollider()
    {
        if(weaponDamageCollider != null)
        {
            weaponDamageCollider.gameObject.SetActive(true);
        }
    }

    public void DisableDamageCollider()
    {
        if(weaponDamageCollider != null)
        {
            weaponDamageCollider.gameObject.SetActive(false);
        }
    }
    
}
