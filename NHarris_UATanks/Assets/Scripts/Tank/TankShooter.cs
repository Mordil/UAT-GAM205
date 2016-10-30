using L4.Unity.Common;
using UnityEngine;

[RequireComponent(typeof(TankSettings), typeof(TankMotor))]
public class TankShooter : BaseScript
{
    [SerializeField]
    private TankSettings _tankSettings;
    [SerializeField]
    private TankMotor _motor;
    [SerializeField]
    private TankController _controller;

    protected override void CheckDependencies()
    {
        base.CheckDependencies();

        this.CheckAndAssignIfDependencyIsNull(ref _tankSettings);
        this.CheckAndAssignIfDependencyIsNull(ref _motor);
        this.CheckAndAssignIfDependencyIsNull(ref _controller);
    }

    /// <summary>
    /// Instantiates a new bullet from the GameObject's TankSettings component and initializes it.
    /// </summary>
    public void Fire()
    {
        BulletSettings bulletSettings = _tankSettings.BulletSettings;

        SpawnBullet(_motor.TransformComponent.forward);

        if (_controller.HasTripleShot)
        {
            var leftBullet = SpawnBullet((_motor.TransformComponent.forward - _motor.transform.right).normalized);
            leftBullet.transform.RotateAround(leftBullet.transform.position, Vector3.up, -45f);

            var rightBullet = SpawnBullet((_motor.TransformComponent.forward + _motor.transform.right).normalized);
            rightBullet.transform.RotateAround(rightBullet.transform.position, Vector3.up, 45f);
        }
    }

    private GameObject SpawnBullet(Vector3 direction)
    {
        BulletSettings bulletSettings = _tankSettings.BulletSettings;
        
        // instantiate a bullet and keep a reference to the GameObject
        var bullet = Instantiate(
            bulletSettings.Prefab,
            bulletSettings.SpawnPoint.position,
            bulletSettings.SpawnPoint.rotation * bulletSettings.Prefab.transform.rotation) as GameObject;

        // gives the bullet its forward trajectory and owner reference
        bullet.GetComponent<TankBullet>()
            .Initialize(direction, _controller);

        bullet.gameObject.name = this.gameObject.GetHashCode().ToString() + "_Bullet";

        return bullet;
    }
}
