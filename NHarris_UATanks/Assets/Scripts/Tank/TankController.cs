using UnityEngine;

[RequireComponent(typeof(TankSettings))]
public class TankController : MonoBehaviour
{
    protected bool IsDead { get { return CurrentHealth <= 0; } }

    [SerializeField]
    protected int CurrentHealth;
    [SerializeField]
    protected int Points;

    [SerializeField]
    private TankSettings _settings;

    #region Unity Lifecycle
    protected virtual void Start()
    {
        if (_settings == null)
        {
            _settings = GetComponent<TankSettings>();
        }

        CurrentHealth = _settings.MaxHealth;
	}
	
	protected virtual void Update()
    {
        if (IsDead)
        {
            Destroy(this.gameObject);
            // TODO: Add points to GameManager
        }
	}

    protected virtual void OnTriggerEnter(Collider otherObj)
    {
        // if the tank was hit by a bullet
        if (otherObj.gameObject.IsOnSameLayer(ProjectSettings.Layers.Projectiles))
        {
            // handle bullet collision event
            onBulletCollision(otherObj.gameObject.GetComponent<TankBullet>());
        }
    }
    #endregion

    public void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
    }

    public void GainPoints(int amount)
    {
        Points += amount;
    }

    protected virtual void onBulletCollision(TankBullet bullet)
    {
        // if it's not friendly fire
        if (bullet.Owner.gameObject.layer != this.gameObject.layer)
        {
            TakeDamage(bullet.Damage);

            if (IsDead)
            {
                bullet.Owner.GainPoints(_settings.KillValue);
            }
        }
    }
}
