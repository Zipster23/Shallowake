using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    // Character Controller Game Component
    public CharacterController controller;

    // Player Input Controller Script
    private PlayerInputController playerInputController;

    // Speed for movement
    [SerializeField]
    private float speed = 6f;

    // Rate at which character turns to face in direction of movement
    [SerializeField]
    private float turnSmoothTime = 0.1f;

    private float turnSmoothVelcoity;

    private void Awake()
    {
        // Getting reference to PlayerInputController Script
        playerInputController = GetComponent<PlayerInputController>();
    }
    private void Update()
    {
        // Getting direction based on movement input
        Vector3 direction = new Vector3(playerInputController.MovementInputVector.x, 0, playerInputController.MovementInputVector.y).normalized;
        
        // If enough movement input, then move and turn
        if (direction.magnitude >= 0.1f)
        {
            // Get angle to face
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            // Smooth turn angle
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelcoity, turnSmoothTime);

            // Rotate player
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Move the player
            controller.Move(direction * speed * Time.deltaTime);
        }
    }
}
