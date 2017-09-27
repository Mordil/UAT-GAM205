using L4.Unity.Common;
using UnityEngine;

using TankPowerupAgent = UATTanks.Tank.Components.PowerupAgent;

[RequireComponent(typeof(TankSettings), typeof(TankMovementAgent), typeof(TankShootingAgent))]
public class InputControllerBase : BaseScript
{
    /// <summary>
    /// The UnityEngine.Time.time value representation of time.
    /// </summary>
    protected float TimeLastFired;

    [SerializeField]
    protected TankSettings Settings;
    [SerializeField]
    protected TankMovementAgent MotorComponent;
    [SerializeField]
    protected TankShootingAgent ShooterComponent;
    [SerializeField]
    protected TankPowerupAgent PowerupComponent;
    [SerializeField]
    protected Transform MyTransform;
    
    /// <summary>
    /// Fires a bullet with the ShooterComponent.
    /// </summary>
    protected void Shoot()
    {
        if (PowerupComponent != null && PowerupComponent.HasTripleShot)
        {
            ShooterComponent.FireTripleShot();
        }
        else
        {
            ShooterComponent.FireSingleShot();
        }

        TimeLastFired = Time.time;
    }
    
    /// <summary>
    /// Returns true if the time since last fired is greater than the rate of fire.
    /// </summary>
    /// <returns></returns>
    protected bool CanShoot()
    {
        return (Time.time - TimeLastFired) >= Settings.BulletSettings.RateOfFire;
    }

    /// <summary>
    /// Returns the Vector3.SqrMagnitude result of the target and this object.
    /// </summary>
    /// <param name="target">The target object to calculate distance from.</param>
    /// <returns></returns>
    protected float GetDistanceFromObject(Transform target)
    {
        return Vector3.SqrMagnitude(target.position - MyTransform.position);
    }
}
