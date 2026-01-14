using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Stamina Damage")]
public class TakeStaminaDamageEffect : InstantCharacterEffect
{
    public float staminaDamage;
    public override void ProcessEffect(CharacterStatsManager character)
    {
        CalculateStaminaDamage(character);
    }

    private void CalculateStaminaDamage(CharacterStatsManager character)
    {
        Debug.Log("Character is taking: " + staminaDamage + " Stamina Damage");
        character.CurrentStamina -= staminaDamage;
    }
}
