using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotionManager : CharacterLocomotionManager
{
    
    PlayerManager player;
    public float verticalMovement; // how much the player is pushing up and down
    public float horizontalMovement; // how much the player is pushing left and right
    public float moveAmount; // how much total movement is happening (vertical and horizontal)

    private Vector3 moveDirection;
    // sets "test" integers for walking and running speed
    [SerializeField] float walkingSpeed = 2;
    [SerializeField] float runningSpeed = 5;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>(); // gets a reference to the PlayerManager
    }

    public void HandleAllMovement() // function that will handle all movement (grounded, aerial, etc)
    {

        HandleGroundedMovement();

    }

    private void GetVerticalAndHorizontalInputs()
    {
        verticalMovement = PlayerInputManager.instance.verticalInput;
        horizontalMovement = PlayerInputManager.instance.horizontalInput;
    }

    private void HandleGroundedMovement()
    {
        
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

}
