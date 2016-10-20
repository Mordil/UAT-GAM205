using UnityEngine;

public class AIInputController : InputController
{
    protected override void HandleMovementInput()
    {
        // TODO: Add movement AI
    }

    protected override void HandleRotationInput()
    {
        // TODO: Add Rotation AI
    }

    protected override void HandleShootingInput()
    {
        if (CanShoot())
        {
            Shoot();
        }
    }
}
