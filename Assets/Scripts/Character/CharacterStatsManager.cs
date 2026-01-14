using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    [Header("Status")]
    public bool isDead = false;

    public int currentHealth;

    CharacterManager character; // reference to the CharacterManager component used to check character states (sprinting, dodging, backstepping, etc)

    [HideInInspector] public CharacterEffectsManager characterEffectsManager;

    public int endurance = 1;   // stat variable. The higher the endurance, the more max stamina.

    public int maxStamina = 0;                              // maximum stamina a character can have
    private float staminaRegenerationTimer = 0;             // tracks how long it's been since stamina last decreased to add a delay before stamina starts regenerating
    private float staminaTickTimer = 0;                     // tracks time between each "tick" of stamina regeneration and prevents stamina from regenerating at once
    [SerializeField] float staminaRegenerationDelay = 2;    // how many seconds to wait after stamina is used before it starts regenerating
    [SerializeField] float staminaRegenerationAmount = 2;   // how much stamina to restore with each regeneration tick

    [SerializeField] private float currentStamina = 0;      // The character's current stamina value

    // property that allows controlled access to currentStamina. Let's you run code whenever a value is read (get) or changed (set)
    public float CurrentStamina
    {
        // called when something reads currentStamina
        get {return currentStamina; }  

        // called when something changes currentStamina
        set{
            ResetStaminaRegenTimer(currentStamina, value);  // before updating stamina, reset the regen timer. This ensures stamina won't start regenerating right as it's being used
            PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue(Mathf.RoundToInt(currentStamina), Mathf.RoundToInt(value));  // Updates the UI to show the new stamina value
            currentStamina = value; // updates the actual value
        }

    }

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();   // gets the CharacterManager component used to check the character's current state (e.g. sprinting, dodging, etc)
        characterEffectsManager = GetComponent<CharacterEffectsManager>();
    }


    // calculates how much stamina the player should have based on the endurance variable
    public int CalculateStaminaBasedOnEnduranceLevel(int endurance)
    {
        float stamina = 0;  // stamina variable

        // simple equation for how stamina is calculated
        stamina = endurance * 10;

        // convert the float result to an integer and return it
        return Mathf.RoundToInt(stamina);
    }

    // handles the gradual regeneration of stamina over time
    public virtual void RegenerateStamina()
    {
        // Don't regenerate stamina if the character is performing an action
        if(character.isPerformingAction)
        {
            return;
        }


        // Check if this character is a player. If it is a player and the player is sprinting, don't regenerate stamina.
        PlayerManager player = GetComponent<PlayerManager>();
        if(player != null)
        {
            if(player.playerInputManager.isSprinting)
            {
                return;
            }
        }



        staminaRegenerationTimer += Time.deltaTime; // increases the regeneration timer by the time since last frame

        // checks if enough time has passed since stamina was last used (e.g. starts regenerating stamina after x seconds)
        if(staminaRegenerationTimer >= staminaRegenerationDelay)
        {
            // only regenerates stamina if stamina isn't already full
            if(CurrentStamina < maxStamina)
            {
                staminaTickTimer += Time.deltaTime; // controls how often stamina regenerates. Time.deltaTime makes it so that the tick rate is consistent regardless of your FPS

                // every 0.1 seconds, adds staminaRegnerationAmount.
                if(staminaTickTimer >= 0.1f)
                {
                    staminaTickTimer = 0;
                    CurrentStamina += staminaRegenerationAmount;
                }
            }
        }
    }

    // resets the regeneration timer when stamina is used. This creates the delay before stamina starts regenerating again
    public virtual void ResetStaminaRegenTimer(float oldValue, float newValue)
    {
        // only reset the timer if stamina is decreased 
        if(newValue < oldValue)
        {
            staminaRegenerationTimer = 0;   // resets the timer back to 0 so that the player has to wait the full staminaRegenerationDelay before stamina starts regenerating again
        }
    }

}


