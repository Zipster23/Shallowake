using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;

    PlayerControls playerControls;

    [SerializeField] Vector2 movementInput;
    public float verticalInput;
    public float horizontalInput;
    public float moveAmount;

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
        // If we are loading into our world scene, enable our player controls
        if(newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
        {
            instance.enabled = true;
        }
        // Otherwise, we must be at the main menu scene. Disable our player controls.
        // This is so the player can't move around in the main menu scene.
        else
        {
            instance.enabled = false;
        }
    }

    private void OnEnable()
    {
        if(playerControls == null) 
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();;
        }

        playerControls.Enable();
    }
        
    private void OnDestroy()
    {
        // if we destroy this object, unsusbcribe from this event
        SceneManager.activeSceneChanged -= OnSceneChange;
    }

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
        HandleMovementInput();
    }

    private void HandleMovementInput()
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
    }

}
