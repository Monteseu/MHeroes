using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponsModel_Library", menuName = "Game Data/Weapons/WeaponsModelLibrary")]
public class WeaponsModelLibrary : ScriptableObject
{
    [SerializeField]
    private List<WeaponModelData> weaponModelsList;
    public List<WeaponModelData> GetWeapons() => weaponModelsList;
    public List<WeaponModelData> GetWeaponsByClass(HeroClass heroClass) => weaponModelsList.FindAll(x => x.compatibleHeroClasses == heroClass);
    public WeaponModelData GetWeaponByID(string id) => weaponModelsList.Find(x => x.ID == id);
}