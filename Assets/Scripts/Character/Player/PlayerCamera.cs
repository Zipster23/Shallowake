using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCamera : MonoBehaviour
{

    public static PlayerCamera instance;
    public PlayerManager player;
    public Camera cameraObject;
    [SerializeField] Transform cameraPivotTransform;

    // Change these to tweak camera performance
    [Header("Camera Settings")]
    private float cameraSmoothSpeed = 1; // The bigger the num, the longer it takes for cam to reach its position
    [SerializeField] float leftAndRightRotationSpeed  = 50;
    [SerializeField] float upAndDownRotationSpeed = 50;
    [SerializeField] float minimumPivot = -30; // lowest point you are able to look down
    [SerializeField] float maximumPivot = 60; // highest point you are able to look up
    [SerializeField] float cameraCollisionRadius = 0.2f;
    [SerializeField] LayerMask collideWithLayers;

    [Header("Camera Values")]
    private Vector3 cameraVelocity;
    private Vector3 cameraObjectPosition;
    [SerializeField] float leftAndRightLookAngle;
    [SerializeField] float upAndDownLookAngle;
    private float cameraZPosition;
    private float targetCameraZPosition;

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
        cameraZPosition = cameraObject.transform.localPosition.z;
    }

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneChange;
    }

    private void OnSceneChange(Scene oldScene, Scene newScene)
    {
        // When we load into the world scene, find the player
        if(newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
        {
            player = FindObjectOfType<PlayerManager>();
        }
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChange;
    }

    private void LateUpdate()
    {
        HandleAllCameraActions();
    }

    public void HandleAllCameraActions()
    {
        if(player != null)
        {
            HandleFollowTarget();
            HandleRotations();
            HandleCollisions();
        }
    }

    private void HandleFollowTarget()
    {
        Vector3 targetCameraZPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
        transform.position = targetCameraZPosition;
    }

    private void HandleRotations()
    {
        leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed) * Time.deltaTime; // Gets your horizontal rotation when you move the mouse/joystick
        upAndDownLookAngle -= (PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed) * Time.deltaTime;         // Gets your vertical rotation when you move the mouse/joystick
        upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot); // Prevents you from looking too far up or down

        Vector3 cameraRotation = Vector3.zero; // Blank variable to hold rotation numbers
        Quaternion targetRotation;             // Make a quaternion variable (used to represent rotation) that will be used to store the final rotation we'll apply

        cameraRotation.y = leftAndRightLookAngle;           // Put the left/right angle into the Y slot (spinning horizontally)
        targetRotation = Quaternion.Euler(cameraRotation);  // Turn those numbers into an actual rotation Unity can use
        transform.rotation = targetRotation;                // Apply this rotation to the camera

        cameraRotation = Vector3.zero;                          // Clear the variable to use it again
        cameraRotation.x = upAndDownLookAngle;                  // Put the up/down angle into the X slot (tilting vertically)
        targetRotation = Quaternion.Euler(cameraRotation);      // Turn those numbers into an actual rotation Unity can use
        cameraPivotTransform.localRotation = targetRotation;    // Apply this rotation to the camera
    }   

    private void HandleCollisions()
    {
        targetCameraZPosition = cameraZPosition;    // Start by assuming the camera should be at it's default distance
        RaycastHit hit;                             // Create a variable to store information if we hit something (a wall, etc)
        Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;    // Calculate the direction from the pivot to the camera
        direction.Normalize();  // Make the direction vector length equal to 1 so it only keeps the direction and disregards the distance

        // Shoots an invisible sphere from the pivot to the camera to check if anything is blocking the view
        if(Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCameraZPosition), collideWithLayers)) 
        {
            float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point); // Measures how far away the obstacle is from the pivot
            targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);                 // Moves the camera closer (makes Z more negative) so it stops just before the obstacle
        } 

        // If the camera would be to close (closer than the collision sphere size), keep it at minimum distance
        if(Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)    
        {
            targetCameraZPosition = -cameraCollisionRadius;
        }

        cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, 0.2f);   // Smoothly move the camera's Z position toward the target position
        cameraObject.transform.localPosition = cameraObjectPosition;                                                // Actually apply the new position to the camera
    }

}
