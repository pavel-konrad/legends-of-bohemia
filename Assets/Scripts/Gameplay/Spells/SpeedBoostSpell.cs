
public class SpeedBoostSpell : SpellBase
{
    public override void Cast(ISpellTarget target)
        {
            target.ModifySpeed(Data.EffectValue, Data.Duration);

        }
}