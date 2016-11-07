using L4.Unity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/*
    NOTE: To see the code for L4.Unity.Common, go to https://github.com/Mordil/Unity-Utility.
*/

[Serializable]
public class PlayerHUDController
{
    public Text LivesLeftText;
    public Text ScoreText;

    public void CheckDependencies()
    {
        if (LivesLeftText == null)
        {
            throw new UnityException("LivesLeftText has not been assigned!");
        }

        if (ScoreText == null)
        {
            throw new UnityException("ScoreText has not been assigned!");
        }
    }
}

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

    public void CheckDependencies()
    {
        if (Slider == null)
        {
            throw new UnityException("Slider has not been assigned!");
        }

        if (FillImage == null)
        {
            throw new UnityException("FillImage has not been assigned!");
        }

        if (FullHealthColor == null)
        {
            throw new UnityException("FullHealthColor has not been assigned!");
        }

        if (ZeroHealthColor == null)
        {
            throw new UnityException("ZeroHealthColor has not been assigned!");
        }
    }
}

[RequireComponent(typeof(TankSettings))]
public class TankController : BaseScript
{
    public const string TOOK_DAMAGE_MESSAGE = "OnTookDamage";

    public bool IsDead { get { return _currentHealth <= 0; } }
    public bool HasTripleShot { get { return _currentPickups.Where(x => x is PowerupTripleShot).Count() > 0; } }
    
    public int CurrentHealth { get { return _currentHealth; } }
    public int CurrentScore { get { return _currentScore; } }

    public TankSettings Settings { get { return _settings; } }

    [SerializeField]
    private int _currentHealth;
    [SerializeField]
    private int _currentScore;
    private float _timeOfLastHealthGain;

    [SerializeField]
    private TankSettings _settings;
    [SerializeField]
    private TankHealthHUDController _healthHUDSettings;
    [SerializeField]
    private PlayerHUDController _playerHUDSettings;

    [ReadOnly]
    [SerializeField]
    private List<IPowerup> _currentPickups;

    #region Unity Lifecycle
    protected override void Start()
    {
        base.Start();

        _currentHealth = _settings.MaxHealth;
        _currentPickups = new List<IPowerup>();

        if (Settings.IsPlayer)
        {
            // sync the UI
            _healthHUDSettings.SyncHealthUI(_currentHealth, Settings.MaxHealth);
            _playerHUDSettings.LivesLeftText.text = GameManager.Instance.CurrentScene.As<MainLevel>().GetLivesRemaining(Settings.ID).ToString();
            _playerHUDSettings.ScoreText.text = "0";
        }
	}

    protected override void Awake()
    {
        // not all tanks will be set up at start, so we call it here explicitly
        Start();
    }

    protected override void Update()
    {
        if (IsDead)
        {
            var deathPrefab = Instantiate(
                Settings.DeathSettings.DeathParticlePrefab,
                this.gameObject.transform.position,
                Settings.DeathSettings.DeathParticlePrefab.transform.rotation) as GameObject;

            Destroy(deathPrefab, deathPrefab.GetComponent<ParticleSystem>().duration);
            Destroy(this.gameObject);

            if (Settings.IsPlayer)
            {
                this.SendMessageUpwards(MainLevel.PLAYER_DIED_MESSAGE, Settings.ID);
            }
        }

        UpdatePickups();
	}

    protected virtual void OnCollisionEnter(Collision otherObj)
    {
        // if the tank was hit by a bullet
        if (otherObj.gameObject.IsOnSameLayer(ProjectSettings.Layers.Projectiles))
        {
            onBulletCollision(otherObj.gameObject.GetComponent<TankBullet>());
        }
    }

    protected virtual void OnTriggerEnter(Collider otherObj)
    {
        if (otherObj.gameObject.IsOnSameLayer(ProjectSettings.Layers.Powerup))
        {
            IPowerup powerup = otherObj.gameObject.GetComponent<IPowerup>();

            powerup.OnPickup(this);

            // if the powerup is an actual pickup that we retain, then we'll add it to maintain
            if (powerup.IsPickup)
            {
                _currentPickups.Add(powerup);
            }
        }
    }
    #endregion

    #region BaseScript
    protected override void CheckDependencies()
    {
        this.CheckAndAssignIfDependencyIsNull(ref _settings);

        if (Settings.IsPlayer)
        {
            _healthHUDSettings.CheckDependencies();
            _playerHUDSettings.CheckDependencies();
        }
    }
    #endregion

    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;

        if (Settings.IsPlayer)
        {
            _healthHUDSettings.SyncHealthUI(_currentHealth, Settings.MaxHealth);
        }
    }

    public void AddHealth(int amount)
    {
        _currentHealth = Mathf.Clamp(_currentHealth + amount, 0, _settings.MaxHealth);

        if (Settings.IsPlayer)
        {
            _healthHUDSettings.SyncHealthUI(_currentHealth, Settings.MaxHealth);
        }
    }

    /// <summary>
    /// Adds the amount provided to the tank's score if it is a player.
    /// </summary>
    /// <param name="amount"></param>
    public void GainPoints(int amount)
    {
        // only players get points
        if (_settings.IsPlayer)
        {
            _currentScore += amount;
            _playerHUDSettings.ScoreText.text = _currentScore.ToString();
        }
    }

    public void RegenerateHealth()
    {
        if (_timeOfLastHealthGain == 0)
        {
            _timeOfLastHealthGain = Time.time;
            return;
        }

        float timeDiff = Time.time - _timeOfLastHealthGain;

        // if it has been at least 1 second
        if (timeDiff >= 1)
        {
            _timeOfLastHealthGain = Time.time;
            _currentHealth += (int)(_settings.HealthRegenRate * timeDiff);
            _currentHealth = Mathf.Clamp(_currentHealth, 0, _settings.MaxHealth);

            if (Settings.IsPlayer)
            {
                _healthHUDSettings.SyncHealthUI(_currentHealth, Settings.MaxHealth);
            }
        }
    }

    protected virtual void onBulletCollision(TankBullet bullet)
    {
        // if it's not friendly fire
        if (bullet.Owner != null &&
            bullet.Owner.gameObject.layer != this.gameObject.layer)
        {
            TakeDamage(bullet.Damage);

            if (IsDead)
            {
                // notify the tank it should probably earn points.
                bullet.Owner.GainPoints(_settings.KillValue);
            }
            else
            {
                this.BroadcastMessage(TankController.TOOK_DAMAGE_MESSAGE, bullet.Owner, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    private void UpdatePickups()
    {
        List<IPowerup> itemsToRemove = new List<IPowerup>();

        // loop through the powerups so that they can receive updates
        foreach (IPowerup powerup in _currentPickups)
        {
            // if the powerup has signaled it is about to expire
            if (powerup.HasExpired)
            {
                itemsToRemove.Add(powerup);
            }
            else
            {
                powerup.OnUpdate(this);
            }
        }

        itemsToRemove.ForEach(powerup =>
        {
            powerup.OnExpire(this);
            _currentPickups.Remove(powerup);
        });
    }
}
