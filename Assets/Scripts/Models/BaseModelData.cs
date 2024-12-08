using UnityEngine;
using UnityEngine.AddressableAssets;
/// <summary>
/// Basic info for any data model.
/// We could argue if the view properties will be common for every data entity (might not),
/// but adding a bridge class would not break the existing prefabs, so we're leaving it like that for now.
/// </summary>
/// 
public class BaseModelData : ScriptableObject
{
    [Header("Identity")]
    public string ID;
    public string Name;
    [Header("View")]
    public AssetReference prefabReference;
    public Sprite Icon;
}
