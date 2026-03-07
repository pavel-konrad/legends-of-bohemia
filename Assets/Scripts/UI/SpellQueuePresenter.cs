using UnityEngine;
using System.Collections.Generic;

public class SpellQueuePresenter : MonoBehaviour, IObserver<SpellEvent>
{
    [SerializeField] private SpellQueueView _view;
    
    private SpellEventManager _spellManager;
    private ISpell _currentSpell;

    public void SetSpellEventManager(SpellEventManager manager)
    {
        _spellManager = manager;
        _spellManager.RegisterObserver(this);
    }

    public void OnDestroy()
    {
        if (_spellManager != null)
        {
            _spellManager.UnregisterObserver(this);
        }
    }

    public void OnNotify(SpellEvent eventData){
        switch(eventData.Event)
        {
            case SpellEventType.QueueChanged:
            UpdateView(eventData.Queue);
            break;

            case SpellEventType.ChargeTick:
            UpdateCharge(eventData.Value);
            break;

            case SpellEventType.DurationTick:
            UpdateDuration(eventData.Value);
            break;
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