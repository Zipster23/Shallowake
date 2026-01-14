using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEffectsManager : MonoBehaviour
{
    // Process Instant Effects (Take Damage, Heal)

    // Process Timed Effects (Poison. Build Ups)

    // Process Static Effects (Adding/Removing Buffs from Talismans etc.)

    CharacterStatsManager character;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterStatsManager>();
    }

    public void ProcessInstantEffect(InstantCharacterEffect effect)
    {
        effect.ProcessEffect(character);
    }
}
