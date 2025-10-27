using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    public Vector2 MovementInputVector { get; private set; }
    public PlayerControls playerControls;

    private InputAction sprint;
    public bool isSprinting { get; private set; }

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnMove(InputValue inputValue)
    {
        MovementInputVector = inputValue.Get<Vector2>();
    }

    public void OnSprint(InputValue value)
    {
        isSprinting = value.isPressed;
    }
    /*
    private void OnEnable()
    {
        sprint = playerControls.Player.Sprint;
        sprint.Enable();
    }

    private void OnDisable()
    {
        sprint.Disable();
    }
    */
}
