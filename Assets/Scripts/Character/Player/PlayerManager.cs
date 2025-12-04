using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : CharacterManager
{
   
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
    [HideInInspector] public PlayerInputManager playerInputManager;

    protected override void Awake()
    {
        base.Awake();

        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();

        playerInputManager = PlayerInputManager.instance;
    }

    protected override void Update()
    {
        base.Update();

        if(playerInputManager == null)
        {
            return;
        }

        playerLocomotionManager.HandleAllMovement();
    }


}
