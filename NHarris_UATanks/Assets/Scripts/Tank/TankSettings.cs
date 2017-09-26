using System;
using UnityEngine;

public class TankSettings : MonoBehaviour
{
    public bool IsPlayer { get { return _isPlayer; } }

    public int ID { get; set; }
    public int MaxHealth { get { return _maxHealth; } }
    public int HealthRegenRate { get { return _healthRegenRate; } }
    public int KillValue { get { return _killValue; } }
    public float RateOfFire { get { return _rateOfFire; } }

    public TankMovementSettings MovementSettings { get { return _movementSettings; } }
    public TankShooterSettings BulletSettings { get { return _bulletSettings; } }
    public TankDeathSettings DeathSettings { get { return _deathSettings; } }

    [SerializeField]
    private bool _isPlayer = false;

    [SerializeField]
    private int _maxHealth = 20;
    [SerializeField]
    [Tooltip("The amount of health (per second) the tank gains during regeneration.")]
    private int _healthRegenRate = 2;
    [SerializeField]
    [Tooltip("The amount of points this tank is worth for killing.")]
    private int _killValue = 5;
    [SerializeField]
    [Tooltip("The number of seconds between each shot.")]
    private float _rateOfFire = 0.5f;

    [SerializeField]
    private TankMovementSettings _movementSettings;
    [SerializeField]
    private TankShooterSettings _bulletSettings;
    [SerializeField]
    private TankDeathSettings _deathSettings;

    public void ModifyStat(int? health = null)
    {
        if (health.HasValue) { _maxHealth += health.Value; }
    }

    private void Awake()
    {
        // assign the default ID of the tank and do a sanity check on if it's a player
        ID = this.gameObject.GetHashCode();
        _isPlayer = GetComponent<PlayerInputController>() != null;
    }
}
