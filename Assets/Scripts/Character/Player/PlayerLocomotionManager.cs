using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotionManager : CharacterLocomotionManager
{
    // reference to player
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
    [SerializeField] float rotationSpeed = 15;

    [Header("Dodge")]
    private Vector3 rollDirection;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>(); // gets a reference to the PlayerManager
    }

    public void HandleAllMovement() // function that will handle all movement (grounded, aerial, etc)
    {

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
        if(!player.canMove)
        {
            return;
        }

        GetVerticalAndHorizontalInputs();

        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement; // move relative to which way the player is facing
        moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement; // adds the left and right movement
        moveDirection.Normalize(); // makes sure the player doesn't move faster diagonally
        moveDirection.y = 0; // keeps movement horizontal

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

    private void HandleRotation()
    {
        if (!player.canRotate)
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

    /*public void HandleSprinting()
    {
        if (player.isPerformingAction)
        {
            player.playerCharacterManager.isSprinting = true;
        }

        if (moveAmount >= 0.5)
        {
            player.playerCharacterManager.isSprinting = true;
        }
        else
        {
            player.playerCharacterManager.isSprinting = false;
        }
    }*/

    public void AttemptToPerformDodge()
    {
        // if we are moving when we want to dodge, then we roll
        if (PlayerInputManager.instance.moveAmount > 0)
        {
            // roll in the direction of camera
            rollDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
            rollDirection += PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontalInput;
            rollDirection.y = 0;
            // normalizing to prevent diagonals
            rollDirection.Normalize();

            // make the player look in the direction of the roll
            Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
            player.transform.rotation = playerRotation;
            
            // Perform roll animation
        }
        // if we are still, then we backstep
        else
        {
            // perform backstep animation
        }

    }

}
