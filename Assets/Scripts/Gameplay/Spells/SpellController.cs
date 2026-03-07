using System.Collections;
using System.Collections.Generic; 
using UnityEngine;

public class SpellController : MonoBehaviour
{
    private ISpellTarget _target;
    private Queue<ISpell> _spellQueue;
    private bool _isProcessing;
    private SpellEventManager _spellManager;

    void Awake()
    {
        _spellManager = GetComponent<SpellEventManager>();
        _target = GetComponent<ISpellTarget>();
        _spellQueue = new Queue<ISpell>();
    }


    public void Enqueue(ISpell spell)
    {
        
        _spellManager.NotifyObservers(new SpellEvent
        {
            Event = SpellEventType.Collected,
            Spell = spell
        });

         _spellQueue.Enqueue(spell);


        _spellManager.NotifyObservers(new SpellEvent
        {
            Event = SpellEventType.QueueChanged,
            Queue = _spellQueue
        });
       
        if (!_isProcessing)
        {
            StartCoroutine(SpellRoutine());
        }       
    }

    private IEnumerator SpellRoutine()
    {
        _isProcessing = true;
        
        while (_spellQueue.Count > 0)
        {
            float elapsed = 0;
            ISpell spell = _spellQueue.Peek();
            
            
            while(spell.Data.ChargeTime > elapsed)
            {
                
                elapsed += Time.deltaTime;
                _spellManager.NotifyObservers(new SpellEvent
                {
                    Event = SpellEventType.ChargeTick,
                    Value = elapsed / spell.Data.ChargeTime
                });
                yield return null;
            }
            spell.Cast(_target);
            _spellManager.NotifyObservers(new SpellEvent
                {
                    Event = SpellEventType.Cast,
                    Spell = spell
                });
            elapsed = 0;
            while (spell.Data.Duration > elapsed)
            {
                elapsed += Time.deltaTime;
                _spellManager.NotifyObservers(new SpellEvent
                {
                    Event = SpellEventType.DurationTick,
                    Value = 1f - (elapsed / spell.Data.Duration),
                    Spell = spell
                });
                yield return null;
            }
            _spellQueue.Dequeue();

            _spellManager.NotifyObservers(new SpellEvent
            {
                Event = SpellEventType.QueueChanged,
                Queue = _spellQueue
            });

        }
        _isProcessing = false;
    }
}
