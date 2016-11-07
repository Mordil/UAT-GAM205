using UnityEngine;
using System.Collections;
using L4.Unity.Common;
using UnityEngine.UI;

public class MainMenu : SceneBase
{
    [Header("Menu Backgrounds")]
    [SerializeField]
    private Image _mainMenuBackground;
    [SerializeField]
    private Image _settingsMenuBackground;

    [Header("Menus Canvases")]
    [SerializeField]
    private Canvas _mainMenuCanvas;
    [SerializeField]
    private Canvas _settingsMenuCanvas;

    protected override void Start()
    {
        base.Start();

        GoToMainMenu();
    }

    protected override void CheckDependencies()
    {
        base.CheckDependencies();

        this.CheckIfDependencyIsNull(_mainMenuCanvas);
        this.CheckIfDependencyIsNull(_settingsMenuCanvas);
    }

    public void GoToMainMenu()
    {
        // switch active background images
        _mainMenuBackground.gameObject.SetActive(true);
        _settingsMenuBackground.gameObject.SetActive(false);

        // switch active menu canvas
        _mainMenuCanvas.gameObject.SetActive(true);
        _settingsMenuCanvas.gameObject.SetActive(false);
    }

    public void GoToSettings()
    {
        // switch active background images
        _mainMenuBackground.gameObject.SetActive(false);
        _settingsMenuBackground.gameObject.SetActive(true);

        // switch active menu canvas
        _mainMenuCanvas.gameObject.SetActive(false);
        _settingsMenuCanvas.gameObject.SetActive(true);
    }
}
