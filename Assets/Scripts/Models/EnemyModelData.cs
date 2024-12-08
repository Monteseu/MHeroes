using UnityEngine;

[CreateAssetMenu(fileName = "EnemyModelData", menuName = "Game Data/Enemies/New Enemy Model")]
public class EnemyModelData : CombatCharacterModelData
{
    [Header("AI Settings")]
    public float detectionRange = 3f;
    [Header("Spawn Settings")]
    public int worldIndex = 0;
    [Header("Reward Settings")]
    public int XPReward = 1;
}

