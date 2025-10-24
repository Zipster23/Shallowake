using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed;
    private PlayerInputController playerInputController;
    private void Awake()
    {
        playerInputController = GetComponent<PlayerInputController>();
    }

    private void Update()
    {
        Vector3 positionChange = new Vector3(playerInputController.MovementInputVector.x, 0, playerInputController.MovementInputVector.y) * Time.deltaTime * speed;

        transform.position += positionChange;
    }
}
