using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Note: This is the last and more quickly done part of the project. Just a simple GUI to select hero and weapon.
// Of course we'd need to adjust the panel to fit the views (Adding scrollviews to fit more heroes and weapons, or re-arranging the panel, etc.)

public class LevelSetupScreenController : MonoBehaviour
{
    List<HeroModelData> heroes;
    List<WeaponModelData> weapons;
    ModelDataService modelDataService;

    [SerializeField]
    SelectionUIView heroUIViewPrefab;
    [SerializeField]
    SelectionUIView weaponUIViewPrefab;
    [SerializeField]
    Transform heroViewsContainer;
    [SerializeField]
    Transform weaponViewsContainer;

    public Action<string, string> OnGameStart;

    string selectedHeroID = "";
    string selectedWeaponID = "";

    [SerializeField]
    Button battleStartButton;

    void Start()
    {
        StartCoroutine(RetrieveLibraries());
        battleStartButton.onClick.AddListener(StartLevel);
    }

    IEnumerator RetrieveLibraries()
    {
        modelDataService = ServicesManager.Get().GetService<ModelDataService>();

        yield return new WaitUntil(() => modelDataService.AreLibrariesReady());

        heroes = modelDataService.GetLibrary<HeroesModelLibrary>().GetHeroes();
        weapons = modelDataService.GetLibrary<WeaponsModelLibrary>().GetWeapons();

        SetupHeroes();
    }

    void SetupHeroes()
    {
        for (int i = 0; i < heroes.Count; i++)
        {
            SelectionUIView newView = Instantiate(heroUIViewPrefab, heroViewsContainer);
            newView.Setup(heroes[i], OnHeroSelected);
        }
    }
    public void OnHeroSelected(BaseModelData data)
    {
        HeroModelData heroData = data as HeroModelData;

        selectedHeroID = heroData.ID;
        SetupHeroWeapons(heroData);
    }

    void SetupHeroWeapons(HeroModelData heroData)
    {
        CleanContainer(weaponViewsContainer);
        List<WeaponModelData> compatibleWeapons = weapons.FindAll(weapon => weapon.compatibleHeroClasses == heroData.Class);
        for (int i = 0; i < compatibleWeapons.Count; i++)
        {
            SelectionUIView newView = Instantiate(weaponUIViewPrefab, weaponViewsContainer);
            newView.Setup(compatibleWeapons[i], OnWeaponSelected);
        }
    }
    public void OnWeaponSelected(BaseModelData data)
    {
        selectedWeaponID = data.ID;
        battleStartButton.gameObject.SetActive(true);
    }

    void StartLevel()
    {
        gameObject.SetActive(false);
        OnGameStart?.Invoke(selectedHeroID, selectedWeaponID);
    }

    void CleanContainer(Transform container)
    {
        foreach (Transform child in container)
            Destroy(child.gameObject);
    }

}
