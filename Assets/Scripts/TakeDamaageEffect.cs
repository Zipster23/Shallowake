using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamaageEffect : InstantCharacterEffect
{
    [Header("Character Causing Damage")]
    public CharacterManager characterCausignDamage;

    public override void ProcessEffect(CharacterStatsManager character)
    {
        base.ProcessEffect(character);
    }
}
