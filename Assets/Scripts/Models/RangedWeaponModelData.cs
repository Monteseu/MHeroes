using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "RangedWeaponModelData", menuName = "Game Data/Weapons/New Ranged Weapon Model")]
public class RangedWeaponModelData : WeaponModelData
{
    // Note: Is it worth to create a whole model to separate the ranged weapons for a single field? I think absolutely yes.
    // we'd also need to create a bool IsRanged, so those are 2 useless properties on all melee weapons.
    public AssetReference projectilePrefabReference;
}
