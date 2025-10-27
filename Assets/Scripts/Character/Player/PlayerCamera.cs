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
        leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed) * Time.deltaTime;
        upAndDownLookAngle -= (PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed) * Time.deltaTime;
        upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

        Vector3 cameraRotation = Vector3.zero;
        Quaternion targetRotation; 

        // Rotate this gameobject left and right
        cameraRotation.y = leftAndRightLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation);
        transform.rotation = targetRotation;

        // Rotate this pivot gameobject up and down
        cameraRotation = Vector3.zero;
        cameraRotation.x = upAndDownLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation);
        cameraPivotTransform.localRotation = targetRotation;
    }   

    private void HandleCollisions()
    {
        targetCameraZPosition = cameraZPosition;
        RaycastHit hit;
        Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
        direction.Normalize();

        if(Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCameraZPosition), collideWithLayers))
        {
            float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
            targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
        } 

        if(Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
        {
            targetCameraZPosition = -cameraCollisionRadius;
        }

        cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, 0.2f);
        cameraObject.transform.localPosition = cameraObjectPosition;
    }

}
