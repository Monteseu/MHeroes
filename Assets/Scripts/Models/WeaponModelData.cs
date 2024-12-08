using System;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponModelData", menuName = "Game Data/Weapons/New Weapon Model")]
public class WeaponModelData : BaseModelData
{
    public float AttackSpeedBonus = 1f;
    public float AttackRangeBonus = 1f;
    public HeroClass compatibleHeroClasses;
}

// Note: I made it a flag so we can assign a weapon to more than onc class. In a WoW style rpg we wouldn't need this.
[Flags]
public enum HeroClass
{
    None = 0,
    Warrior = 1,
    Mage = 2
    // note: Since it's using flags, next value should be =4, =8 etc (because bit magic)
}

