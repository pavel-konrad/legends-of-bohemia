using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Player/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Identity")]
    public string Name;
    public Sprite Icon;
    public Race Race;
    public Class Class;

    [Header("Stats")]
    public float MoveSpeed;
    public float MaxHealth;
    public float MaxEnergy;
    public float AttackPower;
    public bool IsRanged;

    [Header("Audio")]
    public AudioClip[] FootstepSounds;
    public AudioClip SelectSound;
    public AudioClip[] AttackSound;

    [Header("Behavior")]
    public float HealMultiplier    = 1f;
    public float HealSpreadAmount  = 0f;
    public float PoisonMultiplier  = 1f;
    public float PoisonSpreadAmount = 0f;
    public float SpeedMultiplier   = 1f;

    [Header("Prefab")]
    public GameObject PlayerPrefab;

}
