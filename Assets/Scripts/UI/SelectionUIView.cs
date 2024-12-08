using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SelectionUIView : MonoBehaviour
{
    [SerializeField]
    Button button;
    [SerializeField]
    TextMeshProUGUI heroName;
    [SerializeField]
    Image heroIcon;
    BaseModelData model;
    Action<BaseModelData> onModelSelected;
    public void Setup(BaseModelData data, Action<BaseModelData> onModelSelected)
    {
        model = data;
        heroName.text = model.Name;
        heroIcon.sprite = model.Icon;
        this.onModelSelected = onModelSelected;
        button.onClick.AddListener(OnPress);
    }

    void OnPress() => onModelSelected?.Invoke(model);
}
