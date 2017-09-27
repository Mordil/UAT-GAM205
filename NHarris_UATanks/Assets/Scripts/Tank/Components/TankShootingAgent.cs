using UnityEngine;

[RequireComponent(typeof(TankSettings), typeof(AudioSource))]
public class TankShootingAgent : MonoBehaviour, ITankComponent
{
    [SerializeField]
    private Transform _myTransform;
    [SerializeField]
    private AudioSource _shootingAudioSource;
    private TankShootingSettings _settings;

    public void SetUp(TankSettings settings)
    {
        _settings = settings.BulletSettings;
    }

    /// <summary>
    /// Instantiates a new bullet from the GameObject's TankSettings component and initializes it.
    /// </summary>
    public void FireSingleShot()
    {
        _shootingAudioSource.Play();

        SpawnBullet(_myTransform.forward);
    }

    /// <summary>
    /// Instantiates three bullets from the GameObject's TankSettings component and initializes it.
    /// </summary>
    public void FireTripleShot()
    {
        FireSingleShot();

        var leftBullet = SpawnBullet((_myTransform.forward - _myTransform.right).normalized);
        leftBullet.transform.RotateAround(leftBullet.transform.position, Vector3.up, -45f);

        var rightBullet = SpawnBullet((_myTransform.forward + _myTransform.right).normalized);
        rightBullet.transform.RotateAround(rightBullet.transform.position, Vector3.up, 45f);
    }

    private GameObject SpawnBullet(Vector3 direction)
    {        
        // instantiate a bullet and keep a reference to the GameObject
        var bullet = Instantiate(
            _settings.Prefab,
            _settings.SpawnPoint.position,
            _settings.SpawnPoint.rotation * _settings.Prefab.transform.rotation) as GameObject;

        // gives the bullet its forward trajectory and owner reference
        bullet.GetComponent<TankBulletAgent>()
            .Initialize(direction, this.gameObject, _settings.TankBullet);

        bullet.gameObject.name = this.gameObject.GetHashCode().ToString() + "_Bullet";

        return bullet;
    }
}
