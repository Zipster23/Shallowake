using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotionManager : CharacterLocomotionManager
{
    
    PlayerManager player;
    [HideInInspector] public float verticalMovement; // how much the player is pushing up and down
    [HideInInspector] public float horizontalMovement; // how much the player is pushing left and right
    [HideInInspector] public float moveAmount; // how much total movement is happening (vertical and horizontal)

    [Header("Movement Settings")]
    private Vector3 moveDirection;
    private Vector3 targetRotationDirection;
    [SerializeField] float walkingSpeed = 2;
    [SerializeField] float runningSpeed = 5;
    [SerializeField] float sprintingSpeed = 7.5f;
    [SerializeField] float rotationSpeed = 7.5f;
    [SerializeField] int sprintingStaminaCost = 8;

    [Header("Dodge")]
    private Vector3 rollDirection;
    [SerializeField] float dodgeStaminaCost = 25;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>(); // gets a reference to the PlayerManager
    }

    public void HandleAllMovement() // function that will handle all movement (grounded, aerial, etc)
    {
        if (player.isPerformingAction)
        {
            return;
        }

        HandleGroundedMovement();
        HandleRotation();

    }

    private void GetVerticalAndHorizontalInputs()
    {
        verticalMovement = PlayerInputManager.instance.verticalInput;
        horizontalMovement = PlayerInputManager.instance.horizontalInput;
    }

    private void HandleGroundedMovement()
    {
        if (!player.canMove)
        {
            return;
        }

        GetVerticalAndHorizontalInputs();

        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement; // move relative to which way the player is facing
        moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement; // adds the left and right movement
        moveDirection.Normalize(); // makes sure the player doesn't move faster diagonally
        moveDirection.y = 0; // keeps movement horizontal

        if(player.playerInputManager.isSprinting)
        {   
            // move at a running speed
            player.characterController.Move(moveDirection * sprintingSpeed * Time.deltaTime);
        }
        else
        {
            if(PlayerInputManager.instance.moveAmount > 0.5f)
            {
                // move at a running speed
                player.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
            }
            else if(PlayerInputManager.instance.moveAmount <= 0.5f)
            {
                // move at a walking speed
                player.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
            }
        }

    }

    private void HandleRotation()
    {
        if(!player.canRotate)
        {
            return;
        }

        Vector3 targetRotationDirection = Vector3.zero;
        targetRotationDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
        targetRotationDirection = targetRotationDirection + PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
        targetRotationDirection.Normalize();
        targetRotationDirection.y = 0;

        if(targetRotationDirection == Vector3.zero)
        {
            targetRotationDirection = transform.forward;
        }

        Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = targetRotation;
    }

    // method determines whether the player should be sprinting
    public void HandleSprinting()
    {
        // checks if the player is performing an action (e.g. dodging, backstep, attacking)
        if(player.isPerformingAction)
        {
            player.playerInputManager.isSprinting = false;  // set isSprinting to false so the player can't sprint while performing an action
        }

        // If the character has no stamina, stop it from sprinting
        if(player.characterStatsManager.CurrentStamina <= 0)
        {
            player.playerInputManager.isSprinting = false;
            return;
        }

        // checks if the player is moving (0.5 ensures they're at least walking)
        if(PlayerInputManager.instance.moveAmount >= 0.5)
        {
            player.playerInputManager.isSprinting = true;   // set isSprinting to true so the player moves at sprinting speed
        }
        else
        {
            player.playerInputManager.isSprinting = false;  // if barely moving or standing still, set isSprinting to false so they don't move at sprinting speed
        }

        // If sprinting, gradually reduce the player's stamina by the sprintingStaminaCost
        if(player.playerInputManager.isSprinting)
        {
            player.characterStatsManager.CurrentStamina -= sprintingStaminaCost * Time.deltaTime;
        }
    }

    public void AttemptToPerformDodge()
    {
        
        if (player.isPerformingAction)
        {
            return;
        }

        // Check if player has enough stamina to dodge
        if(player.characterStatsManager.CurrentStamina < dodgeStaminaCost)
        {
            return; // not enough stamina, can't dodge
        }

        player.characterStatsManager.CurrentStamina -= dodgeStaminaCost;    // deduct stamina cost for dodging

        player.playerInputManager.isSprinting = false;

        // If we are moving when we attempt to dodge, we perform a roll
        if(PlayerInputManager.instance.moveAmount > 0)
        {
            rollDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
            rollDirection += PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontalInput;

            rollDirection.y = 0;

            Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
            player.transform.rotation = playerRotation;
            // Perform a roll animation
            player.playerAnimatorManager.PlayTargetActionAnimation("Roll_Forward_01", true, true);
        }
        // If we are stationary, we perform a backstep
        else
        {
            player.playerAnimatorManager.PlayTargetActionAnimation("Back_Step_01", true, true);
        }
    }



}
