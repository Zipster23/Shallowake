using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    private PlayerInputController playerInputController;

    private Rigidbody playerRigidBody;

    [SerializeField]
    private float speed = 6f;

    [SerializeField]
    private float turnSmoothTime = 0.1f;

    private float turnSmoothVelcoity;

    private void Awake()
    {
        playerInputController = GetComponent<PlayerInputController>();
        playerRigidBody = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        Vector3 direction = new Vector3(playerInputController.MovementInputVector.x, 0, playerInputController.MovementInputVector.y).normalized;
        
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelcoity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            transform.position += new Vector3(direction.x * speed * Time.deltaTime, 0, direction.z * speed * Time.deltaTime);
        }
    }
}
