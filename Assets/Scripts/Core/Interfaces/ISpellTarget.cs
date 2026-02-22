using UnityEngine;
/*
ISpellTarget is an interface for Spells and their effects on target (Player). It's applied
in SpellBase.cs which contains polymorphic method Cast(), that implement their subclasses on their own.
*/
public interface ISpellTarget
{
    void Heal(float amount, float duration);
    void ModifySpeed(float multiplier, float duration);
    void TakeDamage(float amount);
    void ApplyPoison(float damage, float duration);

}