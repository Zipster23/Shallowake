using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotionManager : CharacterLocomotionManager
{
    
    PlayerManager player;
    public float verticalMovement;
    public float horizontalMovement;
    public float moveAmount;

    private Vector3 moveDirection;
    [SerializeField] float walkingSpeed = 2;
    [SerializeField] float runningSpeed = 5;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    public void HandleAllMovement()
    {

    }

    private void HandleGroundedMovement()
    {
        // Move direction is based off the camera's perspective & movement inputs
        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0;

        if(PlayerInputManager.instance.moveAmount > 0.5f)
        {
            // move at a running speed
        }
        else if(PlayerInputManager.instance.moveAmount <= 0.5f)
        {
            // move at a walking speed

        }
    }

}
