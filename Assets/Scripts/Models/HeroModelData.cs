using UnityEngine;

[CreateAssetMenu(fileName = "HeroModelData", menuName = "Game Data/Heroes/New Hero Model")]
public class HeroModelData : CombatCharacterModelData
{
    [Header("Customization Settings")]
    // Let's add some customization like a good old RPG.
    public HeroClass Class;
    [Header("Progression Settings")]
    public int RequiredXPPerLevel = 5;
}

