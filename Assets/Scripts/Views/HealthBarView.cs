using UnityEngine;
using UnityEngine.UI;

public class HealthBarView : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField]
    Vector3 lookingVector;

    public void SetHealthFill(float normalizedValue) => healthSlider.value = Mathf.Clamp01(normalizedValue);
    // If the camera rotates or change its height, we should mofify this.
    void LateUpdate() => transform.rotation = Quaternion.LookRotation(lookingVector);
}
