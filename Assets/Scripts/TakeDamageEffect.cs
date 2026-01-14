using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Damage")]
public class TakeDamageEffect : InstantCharacterEffect
{
    [Header("Character Causing Damage")]
    public CharacterManager characterCausignDamage; // if the damage is caused by another characterse attack it will be stored here

    [Header("Damage")]
    public float physicalDamage = 0; // (In the future will be split into "Standard", "Strike", "Slash", and "Pierce"
    public float magicDamage = 0;
    public float fireDamage = 0;
    public float lightningDamage = 0;
    public float holyDamage = 0;

    [Header("Final Damage")]
    private int finalDamageDealt = 0; // THe damage the character takes after ALL calculations have been made

    [Header("Poise")]
    public float poiseDamage = 0;
    public bool poiseIsBroken = false; //If a character's poise is broken, they will be "Stunned" and play a damage animation

    // (TO DO) Build Ups
    // Build up effect amounts

    [Header("Animation")]
    public bool playDamageAnimation = true;
    public bool manuallySelectDamageAnimation = false;
    public string damageAnimation;

    [Header("Sound FX")]
    public bool willPlayDamageSFX = true;
    public AudioClip elementalDamageSoundFX; // Used on top of regular SFX if there is elemental damage present

    [Header("Direction Damage Taken From")]
    public float angleHitFrom; // Used to determine what damage animation to play (move backwards, to the left, to the right etc.
    public Vector3 contactPoint; // Used to determine where the Blood FX should instantiate


    public override void ProcessEffect(CharacterStatsManager character)
    {
        base.ProcessEffect(character);

        if (character.isDead)
        {
            return;
        }

        // Check for "Invulnerability"

        CalculateDamage(character);
        // Check Which direction damage came from
        // Play a damage animation
        // Check for build ups (Poison, Bleed, etc.)
        // Play Damage Sound FFX
        // Play Damage VFX (Blood)

        // IF Character is A.I., Check for new target if character causing damage is present

    }

    private void CalculateDamage(CharacterStatsManager character)
    {

        if (characterCausignDamage != null)
        {
            // Check for damage modifiers and modify base damage (physical/elemental damage buff)
        }

        // Check Character for flat defenses and subtract them from the damage

        // Check character for armor absorptions, and subtract the percentage from the damage

        // Add all damage types together, and apply final damage
        finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lightningDamage + holyDamage);

        if (finalDamageDealt <= 0)
        {
            finalDamageDealt = 1;
        }

        Debug.Log("Final Damage Given: " + finalDamageDealt);

        character.currentHealth -= finalDamageDealt;
        // Calculate poise damage to determine the character will be stunned
    }
}
