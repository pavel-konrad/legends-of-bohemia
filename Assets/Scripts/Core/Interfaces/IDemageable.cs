using UnityEngine;
public interface IDemageable
{
    void TakeDamage(float amount);
    float MaxHealth { get; }

}