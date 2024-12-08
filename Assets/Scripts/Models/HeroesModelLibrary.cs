using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeroModel_Library", menuName = "Game Data/Heroes/HeroesModelLibrary")]
public class HeroesModelLibrary : ScriptableObject
{
    [SerializeField]
    private List<HeroModelData> heroModelsList;
    public List<HeroModelData> GetHeroes() => heroModelsList;
    public HeroModelData GetHeroByID(string id) => heroModelsList.Find(x => x.ID == id);

    // We could Get our heroes by class, stats, id, view...
}