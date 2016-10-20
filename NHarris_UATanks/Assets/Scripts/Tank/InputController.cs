using L4.Unity.Common;
using System;
using UnityEngine;

[RequireComponent(typeof (TankSettings), typeof(TankMotor), typeof(TankShooter))]
public class InputController : BaseScript
{
    protected DateTime TimeLastFired;
    [SerializeField]
    protected TankSettings Settings;
    [SerializeField]
    protected TankMotor MotorComponent;
    [SerializeField]
    protected TankShooter ShooterComponent;
	
	protected override void Update()
    {
        HandleMovementInput();
        HandleRotationInput();
        HandleShootingInput();
    }

    protected override void CheckDependencies()
    {
        base.CheckDependencies();

        this.CheckAndAssignIfDependencyIsNull(ref Settings);
        this.CheckAndAssignIfDependencyIsNull(ref MotorComponent);
        this.CheckAndAssignIfDependencyIsNull(ref ShooterComponent);
    }

    // Checks and sends input for positional movement.
    protected virtual void HandleMovementInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            MotorComponent.Move(Settings.MovementSettings.Forward);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            MotorComponent.Move(-Settings.MovementSettings.Backward);
        }
    }

    // Checks and sends input for rotational movement.
    protected virtual void HandleRotationInput()
    {
        if (Input.GetKey(KeyCode.A))
        {
            MotorComponent.Rotate(-Settings.MovementSettings.Rotation);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            MotorComponent.Rotate(Settings.MovementSettings.Rotation);
        }
    }

    // Handles all logic for shooting input
    protected virtual void HandleShootingInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (CanShoot())
            {
                Shoot();
            }
        }
    }

    // Fires a bullet with the ShooterComponent.
    protected void Shoot()
    {
        ShooterComponent.Fire();
        TimeLastFired = DateTime.Now;
    }

    // Returns true if the time since last fired is greater than the rate of fire.
    protected bool CanShoot()
    {
        return (DateTime.Now - TimeLastFired).Seconds >= Settings.RateOfFire;
    }
}
