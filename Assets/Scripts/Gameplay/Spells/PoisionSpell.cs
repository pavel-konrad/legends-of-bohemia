
public class PoisonSpell : SpellBase
{
    public override void Cast(ISpellTarget target)
        {
            target.ApplyPoison(Data.EffectValue, Data.Duration);

        }
}