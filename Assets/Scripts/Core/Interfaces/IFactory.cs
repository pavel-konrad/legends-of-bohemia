using UnityEngine;
public interface IFactory<Tkey, Tproduct>
{
    Tproduct Create(Tkey key);
}
