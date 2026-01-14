using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectsManager : CharacterEffectsManager
{
    [Header("Debug Delete later")]
    [SerializeField] InstantCharacterEffect effectToTest;
    [SerializeField] bool processEffect = false;

    private void Update()
    {
        if(processEffect)
        {
            processEffect = false;

            InstantCharacterEffect effect = Instantiate(effectToTest);

            ProcessInstantEffect(effect);
        }
    }
}
