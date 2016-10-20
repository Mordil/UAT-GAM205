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

        // instantiate a bullet and keep a reference to the GameObject
        var bullet = Instantiate(bulletSettings.Prefab,
                                bulletSettings.SpawnPoint.position,
                                bulletSettings.SpawnPoint.rotation * bulletSettings.Prefab.transform.rotation) as GameObject;

        // gives the bullet its forward trajectory and owner reference
        bullet.GetComponent<TankBullet>()
            .Initialize(_motor.ForwardVector, _controller);
    }
}
