using UnityEngine;
using System;
public interface ISpell : IGridOccupant
{
    SpellData Data {get;}
    bool IsCharged {get;}
    void Cast(ISpellTarget target);
    // void Collect(); 

}