using UnityEngine;

public class PlayerStatsPresenter : MonoBehaviour
{
    [SerializeField] private PlayerBarView _view;
    private PlayerController _player;

    public void SetPlayer(PlayerController player)
    {
        _player = player;
        _player.OnHealthChanged += UpdateHealth;
        _player.OnEnergyChanged += UpdateEnergy;

        UpdateHealth(_player.CurrentHealth, _player.MaxHealth);
        UpdateEnergy(_player.CurrentEnergy, _player.MaxEnergy);
    }

    private void OnDestroy()
    {
        if (_player != null)
        {
            _player.OnHealthChanged -= UpdateHealth;
            _player.OnEnergyChanged -= UpdateEnergy;
        }
    }

    private void UpdateHealth(float current, float max)
    {
        _view.UpdateHealth(current / max);
    }

    private void UpdateEnergy(float current, float max)
    {
        _view.UpdateEnergy(current / max);
    }
}