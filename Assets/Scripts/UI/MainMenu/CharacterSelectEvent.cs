public enum CharacterSelectEventType
{
    PlayerSlotClicked,
    RaceSelected,
    ClassSelected,
    PlayerConfirmed,
    SelectionReset,
    PlayerReset,
    PanelClosed
}

public class CharacterSelectEvent
{
    public CharacterSelectEventType Event;
    public Race Race;
    public PlayerData Player;
}
