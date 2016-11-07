using L4.Unity.Common;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenuController : BaseScript
{
    [Serializable]
    private class ScoreboardSettings
    {
        public Text HighScoreText;
        public GameObject[] PlayerScorePanels = new GameObject[4];
        public Text[] PlayerScoreText = new Text[4];

        public void CheckDependencies()
        {
            if (HighScoreText == null)
            {
                throw new UnityException("HighScoreText reference not set!");
            }

            foreach (GameObject panel in PlayerScorePanels)
            {
                if (panel == null)
                {
                    throw new UnityException("Player panel reference not set!");
                }
                panel.SetActive(false);
            }

            foreach (Text label in PlayerScoreText)
            {
                if (label == null)
                {
                    throw new UnityException("Player score text reference not set!");
                }
            }

            for (int i = 0; i < GameManager.Instance.Settings.NumberOfPlayers; i++)
            {
                PlayerScorePanels[i].SetActive(true);
                PlayerScoreText[i].text = GameManager.Instance.CurrentScene.As<MainLevel>().GetScore(i + 1).ToString();
            }
        }
    }

    [SerializeField]
    private ScoreboardSettings _scoreboard;

    protected override void Awake()
    {
        Start();
    }

    protected override void CheckDependencies()
    {
        _scoreboard.CheckDependencies();
    }

    public void OnRestartButtonClick()
    {
        SceneManager.LoadScene((int)ProjectSettings.Levels.MainGameplay);
    }

    public void OnMainMenuButtonClick()
    {
        GameManager.Instance.GoToLevel(ProjectSettings.Levels.MainMenu);
    }
}
