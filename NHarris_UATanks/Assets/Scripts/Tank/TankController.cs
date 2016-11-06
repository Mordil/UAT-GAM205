using L4.Unity.Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
    NOTE: To see the code for L4.Unity.Common, go to https://github.com/Mordil/Unity-Utility.
*/

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

    [ReadOnly]
    [SerializeField]
    private List<IPowerup> _currentPickups;

    #region Unity Lifecycle
    protected override void Start()
    {
        base.Start();

        _currentHealth = _settings.MaxHealth;
        _currentPickups = new List<IPowerup>();
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
    }
    #endregion

    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;
    }

    public void AddHealth(int amount)
    {
        _currentHealth = Mathf.Clamp(_currentHealth + amount, 0, _settings.MaxHealth);
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
