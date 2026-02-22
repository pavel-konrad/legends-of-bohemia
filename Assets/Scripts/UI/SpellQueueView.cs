using UnityEngine;
using System.Collections.Generic;

public class SpellQueueView : MonoBehaviour
{

    [SerializeField] private List<SpellSlotView> _slots;


    public void UpdateQueue(Queue<ISpell> queue)
    {
        foreach (var slot in _slots)
            slot.Hide();
        
        int index = 0;
        foreach (var spell in queue)
        {
            if (index >= _slots.Count) break;
            _slots[index].Show(spell.Data);
            index++;
        }
    }
}