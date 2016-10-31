using L4.Unity.Common;

public class GameManager : AppManagerBase<GameManager>
{
    // Stub work. All level logic are in SceneBase derivatives.
    // The GameManger is for application level settings like audio, graphics, cheats enabled, etc.
    // Any actual game logic is in their respective SceneBase level components.

    public enum Level { Main }

    public int NumberOfPlayers { get { return _numberOfPlayers; } }

    private int _numberOfPlayers = 1;

    private Level _currentLevel;

    public void GoToLevel(Level newLevel)
    {
        _currentLevel = newLevel;
        // TODO: Handle level changes
    }
}
