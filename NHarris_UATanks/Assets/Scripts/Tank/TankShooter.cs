using UnityEngine;

[RequireComponent(typeof(TankSettings), typeof(TankMotor), typeof(TankController))]
public class TankShooter : MonoBehaviour
{
    [SerializeField]
    private TankSettings _tankSettings;
    [SerializeField]
    private TankMotor _motor;
    [SerializeField]
    private TankController _controller;

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

        if (_controller == null)
        {
            _controller = GetComponent<TankController>();
        }
	}

    /// <summary>
    /// Instantiates a new bullet from the GameObject's TankSettings component and initializes it.
    /// </summary>
    public void Fire()
    {
        BulletSettings bulletSettings = _tankSettings.BulletSettings;

        var bullet = Instantiate(bulletSettings.Prefab,
                                bulletSettings.SpawnPoint.position,
                                bulletSettings.SpawnPoint.rotation * bulletSettings.Prefab.transform.rotation) as GameObject;

        bullet.GetComponent<TankBullet>()
            .Initialize(_motor.ForwardVector, _controller);
    }
}
