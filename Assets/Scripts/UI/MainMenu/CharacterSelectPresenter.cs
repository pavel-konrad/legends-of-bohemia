using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class CharacterSelectPresenter : MonoBehaviour
{
    [SerializeField] private PlayerDataRegistry _registry;
    [SerializeField] private PlayerConfig _playerConfig;
    [SerializeField] private GameSettingsConfig _settings;
    [SerializeField] private CharacterSelectEventManager _eventManager;
    [SerializeField] private Transform _classContainer;
    [SerializeField] private GameObject _classButtonPrefab;

	[Header("Class Panel")]
	[SerializeField] private GameObject _classPanel;
	[SerializeField] private TMP_Text _raceTitle;
	[SerializeField] private TMP_Text _raceDescription;

	[Header("Player Preview")]
	[SerializeField] private TMP_Text _playerName;
	[SerializeField] private Image _playerIcon;
	[SerializeField] private Slider _healthSlider;
	[SerializeField] private Slider _energySlider;
	[SerializeField] private Slider _speedSlider;
	[SerializeField] private Slider _attackSlider;

	[Header("Player Slot")]
	[SerializeField] private GameObject _placeholder;

	[Header("Navigation")]
	[SerializeField] private GameObject _startButton;
	[SerializeField] private GameObject _loadingScreen;
	[SerializeField] private Slider _loadingBar;

	private Race _selectedRace;
	private PlayerData _selectedPlayer;
	private bool _isConfirmed;

	void Awake()
	{
		_startButton.SetActive(false);
	}

	public void OnPlayerSlotClicked()
	{
		_eventManager?.NotifyObservers(new CharacterSelectEvent
		{
			Event = CharacterSelectEventType.PlayerSlotClicked
		});
	}

	public void OnRaceSelected(Race race)
	{
		_selectedRace = race;

		RaceData raceData = _registry.GetRaceData(race);
		_raceTitle.text = raceData.DisplayName;
		_raceDescription.text = raceData.Description;

		foreach (Transform child in _classContainer)
			Destroy(child.gameObject);

		List<PlayerData> available = _registry.GetByRace(race);
		foreach (PlayerData data in available)
		{
			GameObject btn = Instantiate(_classButtonPrefab, _classContainer);
			btn.GetComponentInChildren<TMP_Text>().text = data.Name;
			btn.transform.Find("Icon").GetComponent<Image>().sprite = data.Icon;
			btn.GetComponent<Button>().onClick.AddListener(() => OnClassSelected(data));
		}

		_eventManager?.NotifyObservers(new CharacterSelectEvent
		{
			Event = CharacterSelectEventType.RaceSelected,
			Race = race
		});
	}

	public void OnClassSelected(PlayerData data)
	{
		_selectedPlayer = data;
		_placeholder.SetActive(false);

		_playerName.text = data.Name;
		_playerIcon.sprite = data.Icon;
		_healthSlider.maxValue = _settings.MaxHealth;
		_healthSlider.value = data.MaxHealth;
		_energySlider.maxValue = _settings.MaxEnergy;
		_energySlider.value = data.MaxEnergy;
		_speedSlider.maxValue = _settings.MaxMoveSpeed;
		_speedSlider.value = data.MoveSpeed;
		_attackSlider.maxValue = _settings.MaxAttackPower;
		_attackSlider.value = data.AttackPower;
		_startButton.SetActive(true);

		_eventManager.NotifyObservers(new CharacterSelectEvent
		{
			Event = CharacterSelectEventType.ClassSelected,
			Player = data
		});
	}

	public void OnConfirm()
	{
		if (_selectedPlayer == null) return;

		if (!_isConfirmed)
		{
			_isConfirmed = true;
			_playerConfig.SelectedPlayer = _selectedPlayer;
			_eventManager?.NotifyObservers(new CharacterSelectEvent
			{
				Event = CharacterSelectEventType.PlayerConfirmed,
				Player = _selectedPlayer
			});
		}
		else
		{
			_isConfirmed = false;
			_playerConfig.SelectedPlayer = null;
			_eventManager?.NotifyObservers(new CharacterSelectEvent
			{
				Event = CharacterSelectEventType.SelectionReset
			});
		}
	}

	public void OnStartGame()
	{
		StartCoroutine(LoadGameScene());
	}

	public void OnPlayerReset()
	{
		_isConfirmed = false;
		_selectedPlayer = null;
		_selectedRace = default;
		_playerConfig.SelectedPlayer = null;

		_playerName.text = string.Empty;
		_playerIcon.sprite = null;
		_healthSlider.value = 0;
		_energySlider.value = 0;
		_speedSlider.value = 0;
		_attackSlider.value = 0;

		_placeholder.SetActive(true);

		_eventManager?.NotifyObservers(new CharacterSelectEvent
		{
			Event = CharacterSelectEventType.PlayerReset
		});
	}

	public void OnPanelClosed()
	{
		
		_eventManager?.NotifyObservers(new CharacterSelectEvent
		{
			Event = CharacterSelectEventType.PanelClosed
		});
	}

	private IEnumerator LoadGameScene()
	{
		_loadingScreen.SetActive(true);
		_startButton.SetActive(false);

		AsyncOperation op = SceneManager.LoadSceneAsync("LevelScene");
		op.allowSceneActivation = false;

		while (op.progress < 0.9f)
		{
			_loadingBar.value = op.progress;
			yield return null;
		}

#if UNITY_EDITOR
		float fakeProgress = 0.9f;
		while (fakeProgress < 1f)
		{
			fakeProgress += Time.deltaTime * 0.5f;
			_loadingBar.value = fakeProgress;
			yield return null;
		}
#endif

		_loadingBar.value = 1f;
		yield return new WaitForSeconds(0.5f);
		op.allowSceneActivation = true;
	}

	public void OnSelectHuman() => OnRaceSelected(Race.Human);
	public void OnSelectUndead() => OnRaceSelected(Race.Undead);
	public void OnSelectAngel() => OnRaceSelected(Race.Angel);
	public void OnSelectDevil() => OnRaceSelected(Race.Devil);
}