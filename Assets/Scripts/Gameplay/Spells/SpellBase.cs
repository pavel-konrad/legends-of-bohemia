using UnityEngine;
using System;
using System.Collections;
/*
It's a base Class for all spells. Subclasses like HealSpell and SpeedBoostSpell etc.
implement their own method Cast(). The data comes from scriptable objects in 
data/ScriptableObjects/Spells/ and must be attached in inspector to specific spell. This abstract
Class can't be attached on prefab, instead of their subclasses for every spell.
*/
public abstract class SpellBase : MonoBehaviour, ISpell, ICollectable
{
    public event Action<ICollectable> OnSpellCollected;

    [SerializeField] private SpellData _data;
    public SpellData Data => _data;
    
    public bool IsCharged { get; private set; }
    public Vector2Int GridPosition { get; set; }

    public void OnCollected(IGridOccupant collector)
    {
        OnSpellCollected?.Invoke(this);
        StartCharging();
    }

    public void StartCharging()
    {
        StartCoroutine(ChargeRoutine());
    }

    // public void Collect()
    // {
    //     OnCollected?.Invoke(this);
    // }

    private IEnumerator ChargeRoutine()
    {
        yield return new WaitForSeconds(_data.ChargeTime);
        IsCharged = true;
    }

    public abstract void Cast(ISpellTarget target);
}