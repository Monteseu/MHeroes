using System.Collections;
using UnityEngine;

public class ArenaController : MonoBehaviour
{
    [SerializeField]
    PlayerController playerController;
    [SerializeField]
    EnemySpawner enemySpawner;
    [SerializeField]
    LevelSetupScreenController levelSetupScreenController;

    void Start()
    {
        StartCoroutine(WaitForDataReady());
        levelSetupScreenController.gameObject.SetActive(true);
        levelSetupScreenController.OnGameStart += StartGame;
    }
    public void StartGame(string heroID, string weaponID)
    {
        playerController.Initialize(heroID, weaponID);
    }

    IEnumerator WaitForDataReady()
    {
        yield return new WaitUntil(() => ServicesManager.Get().GetService<ModelDataService>().AreLibrariesReady());
        enemySpawner.Initialize();
    }
}
