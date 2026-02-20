using UnityEngine;
public interface IFactory
{
    GameObject Create(SpellType type);
}
