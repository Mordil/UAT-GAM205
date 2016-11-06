using L4.Unity.Common;
using L4.Unity.Common.Application;
using System;
using UnityEngine;

[Serializable]
public class GameSettings : SettingsBase
{
    [Header("Gameplay")]
    public bool MatchOfTheDay;

    [Range(1, 4)]
    public int NumberOfPlayers = 1;
    public int NumberOfLives = 3;
}

public class GameManager : AppManagerBase<GameManager, GameSettings>
{
    // Stub work. All level logic are in SceneBase derivatives.
    // The GameManger is for application level settings like audio, graphics, cheats enabled, etc.
    // Any actual game logic is in their respective SceneBase level components.

    public enum Level { Main }

    private Level _currentLevel;

    public void GoToLevel(Level newLevel)
    {
        _currentLevel = newLevel;
        // TODO: Handle level changes
    }
}
