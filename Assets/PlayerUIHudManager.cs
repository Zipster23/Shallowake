using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIHudManager : MonoBehaviour
{

    [SerializeField] UI_StatBar staminaBar;     // reference to the stamina bar UI element

    // updates the stamina bar when the player's stamina changes
    public void SetNewStaminaValue(float oldValue, float newValue)
    {
        staminaBar.SetStat(Mathf.RoundToInt(newValue));     // updates the stamina bar's current value
    }

    // sets the maximum capacity of the stamina bar
    public void SetMaxStaminaValue(int maxStamina)
    {
        staminaBar.SetMaxStat(maxStamina);                  // updates the stamina bar's max value
    }

}
