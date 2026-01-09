using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StatBar : MonoBehaviour
{
    
    private Slider slider;

    private float targetValue;
    [SerializeField] private float animationSpeed = 10f;

    protected virtual void Awake()
    {
       slider = GetComponent<Slider>();
    }

    private void Update()
    {
        if(slider.value != targetValue)
        {
            slider.value = Mathf.Lerp(slider.value, targetValue, animationSpeed * Time.deltaTime);

            if(Mathf.Abs(slider.value - targetValue) < 0.01f)
            {
                slider.value = targetValue;
            }
        }
    }

    public virtual void SetStat(int newValue)
    {
        targetValue = newValue;
    }

    public virtual void SetMaxStat(int maxValue)
    {
        slider.maxValue = maxValue;
        targetValue = maxValue;
        slider.value = maxValue;
    }

}
