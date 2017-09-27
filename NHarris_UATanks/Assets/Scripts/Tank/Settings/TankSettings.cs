using UnityEngine;

[CreateAssetMenu(fileName = "New_TankSettings", menuName = "Tank/Settings/Tank")]
public class TankSettings : ScriptableObject
{
    [SerializeField]
    private int _maxHealth = 20;
    public int MaxHealth { get { return _maxHealth; } }

    [SerializeField]
    [Tooltip("The amount of health (per second) the tank gains during regeneration.")]
    private int _healthRegenRate = 2;
    public int HealthRegenRate { get { return _healthRegenRate; } }

    [SerializeField]
    [Tooltip("The amount of points this tank is worth for killing.")]
    private int _killValue = 5;
    public int KillValue { get { return _killValue; } }

    [SerializeField]
    private TankMovementSettings _movementSettings;
    public TankMovementSettings MovementSettings { get { return _movementSettings; } }

    [SerializeField]
    private TankShootingSettings _bulletSettings;
    public TankShootingSettings BulletSettings { get { return _bulletSettings; } }

    [SerializeField]
    private TankDeathSettings _deathSettings;
    public TankDeathSettings DeathSettings { get { return _deathSettings; } }
}
