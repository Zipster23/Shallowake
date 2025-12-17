using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : CharacterManager
{
   
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
    [HideInInspector] public PlayerInputManager playerInputManager;
    [HideInInspector] public CharacterStatsManager characterStatsManager;
    

    protected override void Awake()
    {
        base.Awake();

        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        characterStatsManager = GetComponent<CharacterStatsManager>();

        playerInputManager = PlayerInputManager.instance;
    }

    protected void Start()
    {
        // Calculate max stamina based on endurance stat
        characterStatsManager.maxStamina = characterStatsManager.CalculateStaminaBasedOnEnduranceLevel(characterStatsManager.endurance);

        // Set current stamina to max (start at full stamina)
        characterStatsManager.CurrentStamina = characterStatsManager.maxStamina;

        // Initialize the UI with the max stamina value
        PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(characterStatsManager.maxStamina);
    }

    protected override void Update()
    {
        base.Update();

        if(playerInputManager == null)
        {
            return;
        }

        playerLocomotionManager.HandleAllMovement();

        // Add stamina regeneration to the update loop
        characterStatsManager.RegenerateStamina();
    }


}
