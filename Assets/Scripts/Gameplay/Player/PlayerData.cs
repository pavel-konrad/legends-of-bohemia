using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Player/PlayerData")]
public class PlayerData : ScriptableObject
{
    public string Name;
    public Sprite Icon;
    public Race Race;
    public Class Class;
    public float MoveSpeed;
    public float MaxHealth;
    public float AttackPower;
    public bool IsRanged;
}
