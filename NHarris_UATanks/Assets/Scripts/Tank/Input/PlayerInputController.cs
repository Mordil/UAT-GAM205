using System;
using UnityEngine;

public class PlayerInputController : InputControllerBase
{
    private struct AxisMapping
    {
        private bool _hasBeenSet;
        public bool HasBeenSet { get { return _hasBeenSet; } }

        public string Vertical;
        public string Horizontal;
        public string Shoot;

        public AxisMapping(int playerID)
        {
            switch (playerID)
            {
                case 1:
                    Vertical = ProjectSettings.Axes.PLAYER1_VERTICAL;
                    Horizontal = ProjectSettings.Axes.PLAYER1_HORIZONTAL;
                    Shoot = ProjectSettings.Axes.PLAYER1_SHOOT;
                    break;

                case 2:
                    Vertical = ProjectSettings.Axes.PLAYER2_VERTICAL;
                    Horizontal = ProjectSettings.Axes.PLAYER2_HORIZONTAL;
                    Shoot = ProjectSettings.Axes.PLAYER2_SHOOT;
                    break;
                    
                case 3:
                    Vertical = ProjectSettings.Axes.PLAYER3_VERTICAL;
                    Horizontal = ProjectSettings.Axes.PLAYER3_HORIZONTAL;
                    Shoot = ProjectSettings.Axes.PLAYER3_SHOOT;
                    break;

                case 4:
                    Vertical = ProjectSettings.Axes.PLAYER4_VERTICAL;
                    Horizontal = ProjectSettings.Axes.PLAYER4_HORIZONTAL;
                    Shoot = ProjectSettings.Axes.PLAYER4_SHOOT;
                    break;

                default:
                    throw new IndexOutOfRangeException(string.Format("Max players allowed is 4: Recieved player ID {0}", playerID));
            }

            _hasBeenSet = true;
        }
    }
    
    private AxisMapping _axisMapping;

    protected override void Update()
    {
        if (!_axisMapping.HasBeenSet)
        {
            _axisMapping = new AxisMapping(Settings.ID);
        }

        HandleMovementInput();
        HandleRotationInput();
        HandleShootingInput();
    }

    // Checks and sends input for positional movement.
    protected virtual void HandleMovementInput()
    {
        float input = Input.GetAxis(_axisMapping.Vertical);

        if (input > 0)
        {
            MotorComponent.Move(Settings.MovementSettings.Forward);
        }
        else if (input < 0)
        {
            MotorComponent.Move(-Settings.MovementSettings.Backward);
        }
    }

    // Checks and sends input for rotational movement.
    protected virtual void HandleRotationInput()
    {
        float input = Input.GetAxis(_axisMapping.Horizontal);

        if (input < 0)
        {
            MotorComponent.Rotate(-Settings.MovementSettings.Rotation);
        }
        else if (input > 0)
        {
            MotorComponent.Rotate(Settings.MovementSettings.Rotation);
        }
    }

    // Handles all logic for shooting input
    protected virtual void HandleShootingInput()
    {
        if (Input.GetAxis(_axisMapping.Shoot) > 0)
        {
            if (CanShoot())
            {
                Shoot();
            }
        }
    }
}
