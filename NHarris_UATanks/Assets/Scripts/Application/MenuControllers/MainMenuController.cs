using L4.Unity.Common;
using UnityEngine;

public class MainMenuController : BaseScript
{
    public void OnPlayButtonClick()
    {
        GameManager.Instance.GoToLevel(ProjectSettings.Levels.MainGameplay);
    }

    public void OnSettingsButtonClick()
    {
        GameManager.Instance.CurrentScene.As<MainMenu>().GoToSettings();
    }

    public void OnQuitButtonClick()
    {
        Application.Quit();
    }
}
