using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackManager : MonoBehaviour
{
    PlayerManager player;

    protected void Awake()
    {
        player = GetComponent<PlayerManager>();
    }

    public void AttemptToPerformAttack()
    {
        if(player.isPerformingAction)
        {
            return;
        }

        Debug.Log("Attemping to Perform Attack");
        player.playerAnimatorManager.PlayTargetActionAnimation("Basic_Attack_01", true, true);
    }

    public void AttemptToPerformParry()
    {
        if(player.isPerformingAction)
        {
            return;
        }

        Debug.Log("Attemping to Perform Parry");
        player.playerAnimatorManager.PlayTargetActionAnimation("Basic_Parry_01", true, true);
    }
        
}
