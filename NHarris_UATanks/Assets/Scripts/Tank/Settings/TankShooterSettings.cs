using UnityEngine;

[CreateAssetMenu(fileName = "New_TankShooterSettings", menuName = "Tank/Settings/Shooting")]
public class TankShooterSettings : ScriptableObject
{
    [SerializeField]
    [Tooltip("The number of seconds between each shot.")]
    [Range(0, 10)]
    private float _rateOfFire = 0.5f;
    public float RateOfFire { get { return _rateOfFire; } }

    [SerializeField]
    private TankBullet _tankBullet;
    public TankBullet TankBullet { get { return _tankBullet; } }

    [SerializeField]
    [Tooltip("Where the bullet is to spawn from.")]
    private Transform _spawnPoint;
    /// <summary>
    /// The spawner's transform.
    /// </summary>
    public Transform SpawnPoint { get { return _spawnPoint; } }

    [SerializeField]
    private GameObject _prefab;
    /// <summary>
    /// Reference to the object's prefab.
    /// </summary>
    public GameObject Prefab { get { return _prefab; } }
}
