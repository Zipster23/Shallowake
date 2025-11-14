using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StatBar : MonoBehaviour
{
    
    private Slider slider;                  // variable to store reference to the stamina bar slider in Unity
    
    protected virtual void Awake()
    {   
        slider = GetComponent<Slider>();    // stores the slider gameobject so that we can change the value later
    }

    // Updates the current value of the stat bar.
    public virtual void SetStat(int newValue)
    {
        slider.value = newValue;
    }

    // Sets the maximum possible value for the stamina bar so it doesn't overflow
    public virtual void SetMaxStat(int maxValue)
    {
        slider.maxValue = maxValue;         // Sets the slider's maximum value
        slider.value = maxValue;            // Initializes the value as the starter value so that whenever you load in, you'll spawn with a full stamina bar.
    }

    

}
