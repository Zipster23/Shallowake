using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimatorManager : MonoBehaviour
{

    CharacterManager character; // reference variable

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>(); // sets the variable to the CharacterManager component.
    }

    // Updates the animator's movement parameters to control which animations play based on player input
    public void UpdateAnimatorMovementParameters(float horizontalMovement, float verticalMovement)
    {
        character.animator.SetFloat("Horizontal", horizontalMovement, 0.1f, Time.deltaTime);    // Sets the "Horizontal" animator parameter to the horizontal input value, smoothing the transition over 0.1 seconds
        character.animator.SetFloat("Vertical", verticalMovement, 0.1f, Time.deltaTime);        // Sets the "Vertical" animator parameter to the vertical input value, smoothing the transition over 0.1 seconds        
    }

}
