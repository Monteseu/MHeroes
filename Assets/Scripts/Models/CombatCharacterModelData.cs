using UnityEngine;

public class CombatCharacterModelData : CharacterModelData
{
    [Header("Combat Base Stats")]
    public float MaxHealthPoints = 10f;
    public float BaseAttackDamage = 10f;
    public float BaseAttackRate = 1f;
    public float BaseAttackRange = 1f;
    public float BaseMoveSpeed = 5f;
}