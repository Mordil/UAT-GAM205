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

    public void SyncHealthUI(int maxHealth)
    {
        UpdateUI((int)Math.Ceiling(Slider.value), maxHealth);
    }

    public void SyncHealthUI(int currentHealth, int maxHealth)
    {
        UpdateUI(currentHealth, maxHealth);
    }

    private void UpdateUI(int currentHealth, int maxHealth)
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

        var healthAgent = GetComponent<HealthAgent>();
        healthAgent.OnGainedHealth.AddListener(currentHealth => UpdateHealthHUD(currentHealth));
        healthAgent.OnTookDamage.AddListener(currentHealth => UpdateHealthHUD(currentHealth));
        healthAgent.OnMaxHealthChanged.AddListener(newMaxHealth => _healthHUDController.SyncHealthUI(newMaxHealth));
        healthAgent.OnKilled.AddListener(() => Die());

        var inputAgent = GetComponent<PlayerInputController>();
        inputAgent.ResetAxisMapping(ID);

        var level = GameManager.Instance.CurrentScene.As<MainLevel>();

        _playerHUDController.LivesLeftText.text = level.GetLivesRemaining(ID).ToString();
        _playerHUDController.ScoreText.text = level.GetScore(ID).ToString();

        UpdateHealthHUD(Settings.MaxHealth);
    }

    protected override void Die()
    {
        base.Die();

        this.SendMessageUpwards(MainLevel.PLAYER_DIED_MESSAGE, ID);
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
