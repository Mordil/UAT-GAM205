using UnityEngine;

public class TankBullet : ScriptableObject
{
    [SerializeField]
    private int _damage;
    public int Damage { get { return _damage; } }
    
    [SerializeField]
    [Tooltip("How long the bullet exists before it will self destroy.")]
    private float _lifespan = 3f;
    public float Lifespan { get { return _lifespan; } }

    [SerializeField]
    [Tooltip("Units (in meters) the bullet moves per second.")]
    private float _movementSpeed = 2f;
    public float MovementSpeed { get { return _movementSpeed; } }

    [SerializeField]
    private float _explosionVFXDuration = 1.5f;
    public float ExplosionVFXDuration { get { return _explosionVFXDuration; } }

    [SerializeField]
    private GameObject _explosionVFXPrefab;
    public GameObject ExplosionVFXPrefab;
}
