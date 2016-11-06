using L4.Unity.Common;
using L4.Unity.Common.Application;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private ProjectSettings.Levels _currentLevel;

    public void GoToLevel(ProjectSettings.Levels newLevel)
    {
        _currentLevel = newLevel;
        SceneManager.LoadScene((int)_currentLevel);
    }
}
