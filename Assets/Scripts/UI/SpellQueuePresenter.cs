using UnityEngine;
using System.Collections.Generic;

public class SpellQueuePresenter : MonoBehaviour
{
    [SerializeField] private SpellQueueView _view;
    
    private SpellController _spellController;
    private ISpell _currentSpell;

    public void SetSpellController(SpellController spell)
    {
        _spellController = spell;
        _spellController.OnQueueChanged += UpdateView;
        _spellController.OnChargeTimeTick += UpdateCharge;
        _spellController.OnDurationTimmerTick += UpdateDuration; 
    }

    private void OnDestroy()
    {
        if (_spellController != null)
        {
            _spellController.OnQueueChanged -= UpdateView;
            _spellController.OnChargeTimeTick -= UpdateCharge;
            _spellController.OnDurationTimmerTick -= UpdateDuration;
        }
    }

    private void UpdateView(Queue<ISpell> queue)
    {
        _currentSpell = queue.Count > 0 ? queue.Peek() : null;
        _view.UpdateQueue(queue);
    }

    private void UpdateCharge(float normalized)
    {
        if (_currentSpell == null) return;
        float remaining = (1f - normalized) * _currentSpell.Data.ChargeTime;
        _view.UpdateActiveTimer(remaining, Color.red);
    }

    private void UpdateDuration(float normalized)
    {
        if (_currentSpell == null) return;
        float remaining = normalized * _currentSpell.Data.Duration;
        _view.UpdateActiveTimer(remaining, Color.white);
    }
}