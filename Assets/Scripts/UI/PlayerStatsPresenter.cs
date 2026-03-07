using UnityEngine;

public class PlayerStatsPresenter : MonoBehaviour, IObserver<PlayerEvent>
{
    [SerializeField] private PlayerBarView _view;
    private PlayerController _player;

    public void SetPlayer(PlayerController player)
    {
        _player = player;
        player.RegisterObserver(this);
        
        // _player.OnHealthChanged += UpdateHealth;
        // _player.OnEnergyChanged += UpdateEnergy;

        UpdateHealth(_player.CurrentHealth, _player.MaxHealth);
        UpdateEnergy(_player.CurrentEnergy, _player.MaxEnergy);
    }

    private void OnDestroy()
    {
        if (_player != null)
        {
            _player.UnregisterObserver(this);
            // _player.OnHealthChanged -= UpdateHealth;
            // _player.OnEnergyChanged -= UpdateEnergy;
        }
    }
    public void OnNotify(PlayerEvent eventData)
    {
        switch(eventData.Event)
        {
            case PlayerEventType.HealthChanged:
                UpdateHealth(eventData.Value, eventData.MaxValue);
                break;
            case PlayerEventType.EnergyChanged:
                UpdateEnergy(eventData.Value, eventData.MaxValue);
                break;
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