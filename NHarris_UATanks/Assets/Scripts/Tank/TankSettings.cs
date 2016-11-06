using System;
using UnityEngine;

[Serializable]
public class MovementSpeeds
{
    public float Forward { get { return _forward; } }
    public float Backward { get { return _backward; } }
    public float Rotation { get { return _rotation; } }

    [SerializeField]
    [Tooltip("Units (in meters) to move per second.")]
    private float _forward = 3f;
    [SerializeField]
    [Tooltip("Units (in meters) to move per second.")]
    private float _backward = 3f;
    [SerializeField]
    [Tooltip("Rotation in degrees the body should rotate.")]
    private float _rotation = 180f;
}

[Serializable]
public class BulletSettings
{
    // See TankBullet for additional details.

    /// <summary>
    /// The spawner's transform.
    /// </summary>
    public Transform SpawnPoint { get { return _spawnPoint; } }

    /// <summary>
    /// Reference to the object's prefab.
    /// </summary>
    public GameObject Prefab { get { return _prefab; } }


    [SerializeField]
    private GameObject _prefab;
    [SerializeField]
    [Tooltip("Where the bullet is to spawn at.")]
    private Transform _spawnPoint;
}

[Serializable]
public class DeathSettings
{
    public GameObject DeathParticlePrefab;
}

public class TankSettings : MonoBehaviour
{
    public bool IsPlayer { get { return _isPlayer; } }

    public int ID { get; set; }
    public int MaxHealth { get { return _maxHealth; } }
    public int HealthRegenRate { get { return _healthRegenRate; } }
    public int KillValue { get { return _killValue; } }
    public float RateOfFire { get { return _rateOfFire; } }

    public MovementSpeeds MovementSettings { get { return _movementSettings; } }
    public BulletSettings BulletSettings { get { return _bulletSettings; } }
    public DeathSettings DeathSettings { get { return _deathSettings; } }

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
    private MovementSpeeds _movementSettings;
    [SerializeField]
    private BulletSettings _bulletSettings;
    [SerializeField]
    private DeathSettings _deathSettings;

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
