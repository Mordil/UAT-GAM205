using L4.Unity.Common;
using UnityEngine;

/*
    NOTE: To see the code for L4.Unity.Common, go to https://github.com/Mordil/Unity-Utility.
*/

[RequireComponent(typeof(TankSettings))]
public class TankController : BaseScript
{
    public const string TOOK_DAMAGE_MESSAGE = "OnTookDamage";

    public bool IsDead { get { return _currentHealth <= 0; } }
    
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

    #region Unity Lifecycle
    protected override void Start()
    {
        base.Start();

        _currentHealth = _settings.MaxHealth;
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
            Destroy(this.gameObject);
        }
	}

    protected virtual void OnCollisionEnter(Collision otherObj)
    {
        // if the tank was hit by a bullet
        if (otherObj.gameObject.IsOnSameLayer(ProjectSettings.Layers.Projectiles))
        {
            onBulletCollision(otherObj.gameObject.GetComponent<TankBullet>());
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
        if (bullet.Owner.gameObject.layer != this.gameObject.layer)
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
}
