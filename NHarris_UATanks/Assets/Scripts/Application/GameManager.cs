using L4.Unity.Common;
using L4.Unity.Common.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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

[Serializable]
public class SaveFile
{
    public bool MatchOfTheDay;

    public int NumberOfPlayers;
    public int NumberOfLives;

    public float AmbientVolume;
    public float MasterVolume;
    public float MusicVolume;
    public float SFXVolume;

    public Dictionary<string, int> Highscores;
}

public class GameManager : AppManagerBase<GameManager, GameSettings>
{
    private const string SAVE_FILE_NAME = "/save.data";

    public Dictionary<string, int> HighScores;

    private ProjectSettings.Levels _currentLevel;

    protected override void Start()
    {
        base.Start();

        LoadSave();
    }

    public void GoToLevel(ProjectSettings.Levels newLevel)
    {
        _currentLevel = newLevel;
        SceneManager.LoadScene((int)_currentLevel);
    }

    public void SaveGame()
    {
        var savefile = new SaveFile()
        {
            MatchOfTheDay = Settings.MatchOfTheDay,
            NumberOfPlayers = Settings.NumberOfPlayers,
            NumberOfLives = Settings.NumberOfLives,
            AmbientVolume = Settings.AmbientVolume,
            MasterVolume = Settings.MasterVolume,
            MusicVolume = Settings.MusicVolume,
            SFXVolume = Settings.SFXVolume,
            Highscores = HighScores
        };

        var formatter = new BinaryFormatter();
        var filestream = File.Create(Application.persistentDataPath + SAVE_FILE_NAME);
        formatter.Serialize(filestream, savefile);
        filestream.Close();
    }

    public void LoadSave()
    {
        if (File.Exists(Application.persistentDataPath + SAVE_FILE_NAME))
        {
            var formatter = new BinaryFormatter();
            var filestream = File.Open(Application.persistentDataPath + SAVE_FILE_NAME, FileMode.Open);
            var save = (SaveFile)formatter.Deserialize(filestream);
            filestream.Close();

            Settings.MatchOfTheDay = save.MatchOfTheDay;
            Settings.NumberOfLives = save.NumberOfLives;
            Settings.NumberOfPlayers = save.NumberOfPlayers;
            Settings.MasterVolume = save.MasterVolume;
            Settings.MusicVolume = save.MusicVolume;
            Settings.SFXVolume = save.SFXVolume;
            Settings.AmbientVolume = save.AmbientVolume;

            HighScores = save.Highscores;
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
