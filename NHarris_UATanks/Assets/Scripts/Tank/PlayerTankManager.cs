using L4.Unity.Common;
using System;
using UnityEngine;
using UnityEngine.UI;

/*
    NOTE: To see the code for L4.Unity.Common, go to https://github.com/Mordil/Unity-Utility.
*/

[Serializable]
public class TankHealthHUDController
{
    public Slider Slider;
    public Image FillImage;
    public Color FullHealthColor = Color.green;
    public Color ZeroHealthColor = Color.red;

    public void SyncHealthUI(int currentHealth, int maxHealth)
    {
        Slider.value = currentHealth;
        Slider.maxValue = maxHealth;
        FillImage.color = Color.Lerp(ZeroHealthColor, FullHealthColor, currentHealth / maxHealth);
    }
}

[Serializable]
public class PlayerHUDController
{
    public Text LivesLeftText;
    public Text ScoreText;
}

public class PlayerTankManager : BaseTankManager
{
    [SerializeField]
    private TankHealthHUDController _healthHUDController;
    [SerializeField]
    private PlayerHUDController _playerHUDController;

    protected override void Awake()
    {
        base.Awake();

        var level = GameManager.Instance.CurrentScene.As<MainLevel>();

        UpdateHealthHUD(Settings.MaxHealth);
        _playerHUDController.LivesLeftText.text = level.GetLivesRemaining(ID).ToString();
        _playerHUDController.ScoreText.text = level.GetScore(ID).ToString();
    }

    protected override void Die()
    {
        base.Die();

        this.SendMessageUpwards(MainLevel.PLAYER_DIED_MESSAGE, ID);
    }

    public void OnTookDamage(int currentHealth)
    {
        UpdateHealthHUD(currentHealth);
    }

    public void OnGainedHealth(int amount)
    {
        UpdateHealthHUD(amount);
    }

    public void OnKilledTarget(int targetValue)
    {
        var level = GameManager.Instance.CurrentScene.As<MainLevel>();
        level.AddScore(targetValue, ID);

        _playerHUDController.ScoreText.text = level.GetScore(ID).ToString();
    }

    private void UpdateHealthHUD(int currentHealth)
    {
        _healthHUDController.SyncHealthUI(currentHealth, Settings.MaxHealth);
    }
}
