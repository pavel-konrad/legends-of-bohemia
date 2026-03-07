public class HealSpell : SpellBase
{
    public override void Cast(ISpellTarget target)
    {
        target.Heal(Data.EffectValue, Data.Duration);

    }
}