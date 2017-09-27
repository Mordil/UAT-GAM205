using UnityEngine;
using UnityEngine.Events;

public class HealthAgent : MonoBehaviour, ITankComponent
{
    public UnityEvent<int> OnTookDamage;

    [SerializeField]
    [ReadOnly]
    private int _currentHealth;
    private int _maxHealth;

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
    }

    public void AddHealth(int amount)
    {
        _currentHealth = Mathf.Clamp(_currentHealth + amount, 0, _maxHealth);
    }

    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;
        OnTookDamage.Invoke(_currentHealth);
    }

    private void OnBulletCollision(TankBulletAgent agent)
    {
        // if it's not friendly fire
        if (agent.Owner != null &&
            agent.Owner.gameObject.layer != this.gameObject.layer)
        {
            TakeDamage(agent.Damage);
        }
    }
}
