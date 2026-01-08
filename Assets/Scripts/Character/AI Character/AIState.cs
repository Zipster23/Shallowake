using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState : ScriptableObject
{
    
    public virtual AIState Tick(AICharacterManager aiCharacter)
    {
        Debug.Log("We are running this state");

        
        return this;
    }

}
