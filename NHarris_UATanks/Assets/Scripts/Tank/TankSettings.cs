using System;
using UnityEngine;

[Serializable]
public class MovementSpeeds
{
    public float Forward { get { return _forward; } }
    public float Backward { get { return _backward; } }
    public float Rotation { get { return _rotation; } }

    [Tooltip("Units (in meters) to move per second.")]
    public float _forward = 3f;
    [Tooltip("Units (in meters) to move per second.")]
    public float _backward = 3f;
    [Tooltip("Rotation in degrees the body should rotate.")]
    public float _rotation = 180f;
}

[Serializable]
public class BulletSettings
{
    /// <summary>
    /// The damage it deals when colliding with damagable objects.
    /// </summary>
    public int Damage { get { return _damage; } }
    /// <summary>
    /// Time (in seconds) the bullet stays alive before self destroying.
    /// </summary>
    public float Lifespan { get { return _lifespan; } }
    /// <summary>
    /// Speed (meters per second) the bullet moves.
    /// </summary>
    public float Speed { get { return _speed; } }

    /// <summary>
    /// The spawner's transform.
    /// </summary>
    public Transform SpawnPoint { get { return _spawnPoint; } }

    /// <summary>
    /// Reference to the object's prefab.
    /// </summary>
    public GameObject Prefab { get { return _prefab; } }

    [SerializeField]
    private int _damage = 10;
    [SerializeField]
    [Tooltip("How long the bullet exists before it will self destroy.")]
    private float _lifespan = 3f;
    [SerializeField]
    [Tooltip("Units (in meters) the bullet moves per second.")]
    private float _speed = 2f;

    [SerializeField]
    private GameObject _prefab;
    [SerializeField]
    [Tooltip("Where the bullet is to spawn at.")]
    private Transform _spawnPoint;
}

public class TankSettings : MonoBehaviour
{
    public int MaxHealth { get { return _maxHealth; } }
    public int KillValue { get { return _killValue; } }
    public float RateOfFire { get { return _rateOfFire; } }

    public MovementSpeeds MovementSettings { get { return _movementSettings; } }
    public BulletSettings BulletSettings { get { return _bulletSettings; } }
    
    [SerializeField]
    private int _maxHealth = 20;
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
}
