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
    // sets "test" integers for walking and running speed
    [SerializeField] float walkingSpeed = 2;
    [SerializeField] float runningSpeed = 5;
    [SerializeField] float sprintingSpeed = 7.5f;
    [SerializeField] float rotationSpeed = 7.5f;
    [SerializeField] int sprintingStaminaCost = 2;

    [Header("Movement Smoothing")]
    [SerializeField] float accelerationTime = 0.1f; // Time to reach max speed. Higher = slower build up.
    private float currentSpeedVelocity; // Reference variable for the math (don't touch this)
    private float currentSpeed; // The actual speed the player is moving right now

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

        // 1. Calculate the Direction (Geometry)
        // We calculate this every frame so the direction is always correct relative to camera
        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDirection += PlayerCamera.instance.transform.right * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0;

        // 2. Determine the Target Speed (Logic)
        float targetSpeed = 0;

        // If there is no input, target speed is 0
        if (PlayerInputManager.instance.moveAmount == 0)
        {
            targetSpeed = 0;
        }
        else
        {
            // If we are sprinting
            if (player.playerInputManager.isSprinting)
            {
                targetSpeed = sprintingSpeed;
            }
            // If we are just running (stick pushed fully)
            else if (PlayerInputManager.instance.moveAmount > 0.5f)
            {
                targetSpeed = runningSpeed;
            }
            // If we are walking (stick pushed lightly)
            else
            {
                targetSpeed = walkingSpeed;
            }
        }

        // 3. Smoothly Calculate the Speed (Physics/Feel)
        // This function gradually changes 'currentSpeed' to match 'targetSpeed'
        // 'accelerationTime' controls how sluggish or snappy the change feels
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref currentSpeedVelocity, accelerationTime);

        // 4. Apply the Movement
        // We use 'currentSpeed' instead of the fixed 'sprintingSpeed'
        player.characterController.Move(moveDirection * currentSpeed * Time.deltaTime);
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
