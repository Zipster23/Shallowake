using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

    public static PlayerCamera instance;
    public PlayerManager player;
    public Camera cameraObject;

    [Header("Camera Settings")]
    private Vector3 cameraVelocity;
    private float cameraSmoothSpeed = 1; // The bigger the num, the longer it takes for cam to reach its position

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
    }

    public void HandleAllCameraActions()
    {
        if(player != null)
        {
            FollowTarget();
        }
    }

    private void FollowTarget()
    {
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
        transform.position = targetCameraPosition;
    }

}
