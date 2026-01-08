using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;
    public PlayerManager player;
    public GameObject playerGameObject;

    PlayerControls playerControls;

    [Header("Camera Movement Input")]
    [SerializeField] Vector2 cameraInput;
    public float cameraVerticalInput;
    public float cameraHorizontalInput;

    [Header("Player Movement Input")]
    [SerializeField] Vector2 movementInput;
    public float verticalInput;
    public float horizontalInput;
    public float moveAmount;

    [Header("Player Action Input")]
    [SerializeField] bool dodgeInput = false;
    [SerializeField] bool sprintInput = false;
    public bool isSprinting = false;
    public bool drawSheatheWeaponInput = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        // When the scene changes, this function is ran
        SceneManager.activeSceneChanged += OnSceneChange;

        instance.enabled = false;
    }

    private void OnSceneChange(Scene oldScene, Scene newScene) 
    {

        if (WorldSaveGameManager.instance != null)
        {

            // If we are loading into our world scene, enable our player controls
            if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
            {
                instance.enabled = true;

                // Find the player when we load into the world scene
                playerGameObject = GameObject.FindWithTag("Player");

                if (playerGameObject != null)
                {
                    player = playerGameObject.GetComponent<PlayerManager>();
                }

            }
            // Otherwise, we must be at the main menu scene. Disable our player controls.
            // This is so the player can't move around in the main menu scene.
            else
            {
                instance.enabled = false;
            }
        }
    }

    private void OnEnable()
    {
        if(playerControls == null) 
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerCamera.Movement.performed += i => cameraInput = i.ReadValue<Vector2>();
            playerControls.PlayerActions.Dodge.performed += i => dodgeInput = true;

            playerControls.PlayerActions.Sprint.performed += i => sprintInput = true;   // holding the input sets the bool to true (sprinting) 
            playerControls.PlayerActions.Sprint.canceled += i => sprintInput = false;   // releasing the input sets the bool to false (stops sprinting) 
            playerControls.PlayerActions.DrawWeapon.performed += i => drawSheatheWeaponInput = true;
        
        }

        playerControls.Enable();
    }
        
    private void OnDestroy()
    {
        // if we destroy this object, unsusbcribe from this event
        SceneManager.activeSceneChanged -= OnSceneChange;
    }

    // if we minimize game, stop adjusting inputs
    private void OnApplicationFocus(bool focus)
    {
        if(enabled)
        {
            if(focus)
            {
                playerControls.Enable();
            }
            else
            {
                playerControls.Disable();
            }
        }
    }

    private void Update()
    {
        HandleAllInputs();
    }

    private void HandleAllInputs()
    {
        HandleCameraMovementInput();
        HandlePlayerMovementInput();
        HandleDodgeInput();
        HandleSprintingInput();
    }

    // Movement
    private void HandlePlayerMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        // returns the absolute value of the num so it's always positive.
        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

        if(moveAmount <= 0.5 && moveAmount > 0) 
        {
            moveAmount = 0.5f;
        }
        else if(moveAmount > 0.5 && moveAmount <= 1)
        {
            moveAmount = 1;
        }

        if(player == null)
            return;

        player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerInputManager.isSprinting);
    }

    private void HandleCameraMovementInput()
    {
        cameraVerticalInput = cameraInput.y;
        cameraHorizontalInput = cameraInput.x;
    }

    // Actions

    private void HandleDodgeInput()
    {
        if(dodgeInput)
        {
            dodgeInput = false;

            // Future Note: Return (Do Nothing) if Menu UI Window is Open
            // Perform a dodge
            player.playerLocomotionManager.AttemptToPerformDodge();
        }
    }

    // checks if the player is holding down the sprint button and handles sprinting logic
    private void HandleSprintingInput()
    {
        // checks if the sprint button is being held down
        if(sprintInput)
        {
            player.playerLocomotionManager.HandleSprinting();   // if the sprint button is held, call the HandleSprinting method
        }
        else
        {   
            player.playerInputManager.isSprinting = false;      // if the sprint button is not being held, set isSprinting to false, making the player return to normal walking speed and animations
        }
    }

    private void HandleDrawSheatheInput()
    {
        if(drawSheatheWeaponInput)
        {
            player.weaponModelInstantiation.LoadWeapon();
        } else
        {
            player.weaponModelInstantiation.UnloadWeapon();
        }
    }
}
