using UnityEngine;

public class PlayerInputController : InputControllerBase
{
	protected override void Update()
    {
        HandleMovementInput();
        HandleRotationInput();
        HandleShootingInput();
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
}
