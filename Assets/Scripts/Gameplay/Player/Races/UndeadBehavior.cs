public class UndeadBehavior : CharacterBehavior
{
    public override void OnHeal(float amount, float duration)
    {
        _player.PoisonEffect(amount * _data.HealMultiplier, duration);
        DrainFromEnemies(amount * _data.HealMultiplier);
        SpreadHeal(amount * _data.HealSpreadAmount, duration);
    }

    public override void OnPoison(float damage, float duration)
        => SpreadPoison(damage * _data.PoisonMultiplier, duration);
}
