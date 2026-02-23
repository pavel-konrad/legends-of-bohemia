
using System.Timers;
using System;
using System.Collections;
using System.Collections.Generic; 
using UnityEngine;

public class SpellController : MonoBehaviour
{
    private ISpellTarget _target;
    private Queue<ISpell> _spellQueue;
    private bool _isProcessing;
    public event Action<Queue<ISpell>> OnQueueChanged;
    public event Action<float> OnChargeTimeTick;
    public event Action<float> OnDurationTimmerTick;
    public event Action<ISpell> OnSpellCast;

    void Awake()
    {
        _target = GetComponent<ISpellTarget>();
        _spellQueue = new Queue<ISpell>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Enqueue(ISpell spell)
    {
        _spellQueue.Enqueue(spell);
        OnQueueChanged?.Invoke(_spellQueue);
        #if UNITY_EDITOR
        Debug.Log($"Enqueue: {spell.Data.Name}, subscribers: {OnQueueChanged?.GetInvocationList().Length ?? 0}");
        #endif
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
                OnChargeTimeTick?.Invoke(elapsed / spell.Data.ChargeTime);
                yield return null;
            }
            spell.Cast(_target);
            OnSpellCast?.Invoke(spell);
            elapsed = 0;
            while (spell.Data.Duration > elapsed)
            {
                elapsed += Time.deltaTime;
                OnDurationTimmerTick?.Invoke(1f - (elapsed / spell.Data.Duration));
                yield return null;
            }
            _spellQueue.Dequeue();
            OnQueueChanged?.Invoke(_spellQueue);

        }
        _isProcessing = false;
    }
}
