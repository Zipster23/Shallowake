using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    
    CharacterManager character;

    public int endurance = 1;

    public int maxStamina = 0;
    private float staminaRegenerationTimer = 0;
    private float staminaTickTimer = 0;
    [SerializeField] float staminaRegenerationDelay = 2;
    [SerializeField] float staminaRegenerationAmount = 2;

    [SerializeField] private float currentStamina = 0;

    public float CurrentStamina
    {
        get {return currentStamina; }

        set{
            ResetStaminaRegenTimer(currentStamina, value);
            PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue(Mathf.RoundToInt(currentStamina), Mathf.RoundToInt(value));
            currentStamina = value;
        }

    }

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }


    public int CalculateStaminaBasedOnEnduranceLevel(int endurance)
    {
        float stamina = 0;

        // equation for how stamina is calculated
        stamina = endurance * 10;

        return Mathf.RoundToInt(stamina);
    }

    public virtual void RegenerateStamina()
    {
        /* ERASE THE COMMENT LATER THIS IS JUST BECAUSE ITS CAUSING AN ERROR
        if(character.isSprinting || character.isPerformingAction)
        {
            return;
        }
        */

        staminaRegenerationTimer += Time.deltaTime;

        if(staminaRegenerationTimer >= staminaRegenerationDelay)
        {
            if(CurrentStamina < maxStamina)
            {
                staminaTickTimer += Time.deltaTime;

                if(staminaTickTimer >= 0.1f)
                {
                    staminaTickTimer = 0;
                    CurrentStamina += staminaRegenerationAmount;
                }
            }
        }
    }

    public virtual void ResetStaminaRegenTimer(float oldValue, float newValue)
    {
        if(newValue < oldValue)
        {
            staminaRegenerationTimer = 0;
        }
    }

}
