using L4.Unity.Common;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TankSettings), typeof(TankMotor), typeof(TankShooter))]
public class InputControllerBase : BaseScript
{
    /// <summary>
    /// The UnityEngine.Time.time value representation of time.
    /// </summary>
    protected float TimeLastFired;

    [SerializeField]
    protected TankSettings Settings;
    [SerializeField]
    protected TankMotor MotorComponent;
    [SerializeField]
    protected TankShooter ShooterComponent;
    [SerializeField]
    protected TankController Controller;
    [SerializeField]
    protected Transform MyTransform;

    [ReadOnly]
    [SerializeField]
    protected List<IPowerup> CurrentPickups;

    protected override void CheckDependencies()
    {
        base.CheckDependencies();

        this.CheckAndAssignIfDependencyIsNull(ref Settings);
        this.CheckAndAssignIfDependencyIsNull(ref MotorComponent);
        this.CheckAndAssignIfDependencyIsNull(ref ShooterComponent);
        this.CheckAndAssignIfDependencyIsNull(ref Controller);
        this.CheckAndAssignIfDependencyIsNull(ref MyTransform, true);
    }

    protected virtual void OnTriggerEnter(Collider otherObj)
    {
        if (otherObj.gameObject.IsOnSameLayer(ProjectSettings.Layers.Powerup))
        {
            IPowerup powerup = otherObj.gameObject.GetComponent<IPowerup>();

            powerup.OnPickup(Controller);

            // if the powerup is an actual pickup that we retain, then we'll add it to maintain
            if (powerup.IsPickup)
            {
                CurrentPickups.Add(powerup);
            }
        }
    }
    
    /// <summary>
    /// Fires a bullet with the ShooterComponent.
    /// </summary>
    protected void Shoot()
    {
        ShooterComponent.Fire();
        TimeLastFired = Time.time;
    }
    
    /// <summary>
    /// Returns true if the time since last fired is greater than the rate of fire.
    /// </summary>
    /// <returns></returns>
    protected bool CanShoot()
    {
        return (Time.time - TimeLastFired) >= Settings.RateOfFire;
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
