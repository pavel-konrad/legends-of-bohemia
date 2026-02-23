using UnityEngine;
using UnityEngine.UI;

public class PlayerBarView : MonoBehaviour
{
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _energySlider;

    public void UpdateHealth(float normalized)
    {
        _healthSlider.value = normalized;
    }
    public void UpdateEnergy(float normalized)
    {
        _energySlider.value = normalized;
    }
}