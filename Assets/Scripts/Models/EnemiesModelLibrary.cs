using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyModel_Library", menuName = "Game Data/Enemies/EnemiesModelLibrary")]
public class EnemiesModelLibrary : ScriptableObject
{
    [SerializeField]
    private List<EnemyModelData> enemyModelsList;
    public List<EnemyModelData> GetAllEnemies() => enemyModelsList;
    public EnemyModelData GetEnemyByID(string id) => enemyModelsList.Find(x => x.ID == id);
    // Will we encounter different enemies on different worlds? levels? Stages?
    public List<EnemyModelData> GetEnemiesByWorldIndex(int index) => enemyModelsList.FindAll(x => x.worldIndex == index);
}