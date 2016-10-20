using L4.Unity.Common;
using UnityEngine;

/*
    NOTE: To see the code for L4.Unity.Common, go to https://github.com/Mordil/Unity-Utility.
*/

[RequireComponent(typeof(TankSettings))]
public class TankController : BaseScript
{
    protected bool IsDead { get { return CurrentHealth <= 0; } }

    [SerializeField]
    protected int CurrentHealth;
    [SerializeField]
    protected int CurrentScore;

    [SerializeField]
    private TankSettings _settings;

    #region Unity Lifecycle
    protected override void Start()
    {
        base.Start();

        CurrentHealth = _settings.MaxHealth;
	}
	
	protected override void Update()
    {
        if (IsDead)
        {
            Destroy(this.gameObject);
        }
	}

    protected virtual void OnTriggerEnter(Collider otherObj)
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
        CurrentHealth -= amount;
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
            CurrentScore += amount;
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
        }
    }
}
