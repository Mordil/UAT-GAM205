using UnityEngine;
using UnityEngine.Events;

public class HealthAgent : MonoBehaviour, ITankComponent
{
    public UnityEvent<int> OnTookDamage;
    public UnityEvent<int> OnGainedHealth;
    public UnityEvent<int> OnMaxHealthChanged;
    public UnityEvent OnKilled;

    [SerializeField]
    [ReadOnly]
    private int _currentHealth;
    private int _maxHealth;
    private int _killValue;

    private void OnCollisionEnter(Collision collision)
    {
        // if the tank was hit by a bullet
        if (collision.gameObject.IsOnSameLayer(ProjectSettings.Layers.Projectiles))
        {
            OnBulletCollision(collision.gameObject.GetComponent<TankBulletAgent>());
        }
    }

    public void SetUp(TankSettings settings)
    {
        _maxHealth = settings.MaxHealth;
        _currentHealth = _maxHealth;
        _killValue = settings.KillValue;
    }

    public void ModifyMaxHealth(int amount)
    {
        _maxHealth += amount;
        OnMaxHealthChanged.Invoke(_maxHealth);
    }

    public void AddHealth(int amount)
    {
        _currentHealth = Mathf.Clamp(_currentHealth + amount, 0, _maxHealth);
        OnGainedHealth.Invoke(_currentHealth);
    }

    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;

        if (_currentHealth <= 0)
        {
            OnKilled.Invoke();
        }
        else
        {
            OnTookDamage.Invoke(_currentHealth);
        }
    }

    private void OnBulletCollision(TankBulletAgent agent)
    {
        // if it's not friendly fire
        if (agent.Owner != null &&
            agent.Owner.gameObject.layer != this.gameObject.layer)
        {
            TakeDamage(agent.Damage);

            if (_currentHealth <= 0)
            {
                agent.Owner.SendMessage(BaseTankManager.KILLED_TARGET_MESSAGE, _killValue, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
