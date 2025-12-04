using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Items/Weapons")]
public class MeleeWeaponItem : Item
{
    // Animation Controller Override

    [Header("Weapon Model")]
    public GameObject weaponModel;

    [Header("Weapon Base Damage")]
    public int physicalDamage = 0;

    // Weapon Guard Absorptions (Blocking Power)

    [Header("Weapon Base Poise Damage")]
    public float poiseDamage = 10;
    // Offensive Poise Bonus When Attacking


    // Weapon Modifiers
    // Light Attack Modifier
    // Heavy Attack Modifier
    // Critical Damage Modifier

    [Header("Stamina Costs")]
    public int baseStaminaCost = 20;
    // Running Attack Stamina Cost Modifier
    // Light Attack Stamina Cost Modifier
    // Heavy Attack Stamina Cost Modifier

    //  Item Based Actions

    // Ash of War

    // Blocking Sounds  
}
