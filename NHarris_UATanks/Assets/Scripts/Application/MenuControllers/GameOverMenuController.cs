using L4.Unity.Common;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenuController : BaseScript
{
    [Serializable]
    private class ScoreboardSettings
    {
        public Text HighScoreValueText;
        public Text HighScoreLabelText;
        public GameObject[] PlayerScorePanels = new GameObject[4];
        public Text[] PlayerScoreText = new Text[4];

        public void CheckDependencies()
        {
            if (HighScoreValueText == null)
            {
                throw new UnityException("HighScoreValueText reference not set!");
            }

            if (HighScoreLabelText == null)
            {
                throw new UnityException("HighScoreLabelText reference not set!");
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

        var highscore = GameManager.Instance.HighScores.Aggregate((left, right) => (left.Value > right.Value) ? left : right);
        _scoreboard.HighScoreLabelText.text = highscore.Key;
        _scoreboard.HighScoreValueText.text = highscore.Value.ToString();
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
