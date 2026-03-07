using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "PlayerDataRegistry", menuName = "Player/PlayerDataRegistry")]
public class PlayerDataRegistry : ScriptableObject
{
	public List<PlayerData> PlayerData;
	public List<RaceData> Races;

	public List<PlayerData> GetByRace(Race race)
	{
		return PlayerData.Where(p => p.Race == race).ToList();
	}

	public RaceData GetRaceData(Race race)
	{
		return Races.Find(r => r.Race == race);
	}
}
