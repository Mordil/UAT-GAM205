using UnityEngine;

[RequireComponent(typeof(TankSettings), typeof(TankMotor))]
public class TankShooter : MonoBehaviour
{
    [SerializeField]
    private TankSettings _tankSettings;
    [SerializeField]
    private TankMotor _motor;

    private	void Start()
    {
	    if (_tankSettings == null)
        {
            _tankSettings = GetComponent<TankSettings>();
        }

        if (_motor == null)
        {
            _motor = GetComponent<TankMotor>();
        }
	}

    /// <summary>
    /// Instantiates a new bullet from the GameObject's TankSettings component and initializes it.
    /// </summary>
    public void Fire()
    {
        BulletSettings settings = _tankSettings.BulletSettings;

        var bullet = Instantiate(settings.Prefab,
                                settings.SpawnPoint.position,
                                settings.SpawnPoint.rotation * settings.Prefab.transform.rotation) as GameObject;
        bullet.GetComponent<TankBullet>()
            .Initialize(settings.Damage, settings.Lifespan);
        bullet.GetComponent<Rigidbody>()
            .AddForce(_motor.ForwardVector * settings.Speed, ForceMode.Force);
    }
}
