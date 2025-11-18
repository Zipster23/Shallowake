using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIHudManager : MonoBehaviour
{
  
    public Slider staminaBar;

    private int maxStamina = 100;
    private int currentStamina;

    public static StaminaBar instance;

    private void Awake()
    {
        instance = this;
    }

    public void Start
    {
        currentStamina = maxStamina;
        staminaBar.maxValue = maxStamina;
        staminaBar.value = maxStamina;
    }

    public void UseStamina(int amount)
    {
        if(currentStamina - amount >= 0)
        {
            currentStamina -= amount;
            staminaBar.value = currentStamina;
        }
    }

}
