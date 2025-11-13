using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    
    public int CalculateStaminaBasedOnEnduranceLevel(int endurance)
    {
        float stamina = 0;

        // equation for how stamina is calculated
        stamina = endurance * 10;

        return Mathf.RoundToInt(stamina);
    }

}
