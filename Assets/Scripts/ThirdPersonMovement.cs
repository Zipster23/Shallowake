using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    private PlayerInputController _playerInputController;

    [SerializeField]
    private float speed = 6f;

    [SerializeField]
    private float turnSmoothTime = 0.1f;

    private float turnSmoothVelcoity;

    private void Awake()
    {
        _playerInputController = GetComponent<PlayerInputController>();
    }
    private void Update()
    {
        Vector3 direction = new Vector3(_playerInputController.MovementInputVector.x, 0, _playerInputController.MovementInputVector.y).normalized;
        
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelcoity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            controller.Move(direction * speed * Time.deltaTime);
        }
    }
}
