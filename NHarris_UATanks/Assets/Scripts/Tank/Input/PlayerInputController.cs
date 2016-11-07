using System;
using UnityEngine;

public class PlayerInputController : InputControllerBase
{
    private struct KeyMapping
    {
        private bool _hasBeenSet;
        public bool HasBeenSet { get { return _hasBeenSet; } }

        public KeyCode Forward;
        public KeyCode Backward;
        public KeyCode RotateLeft;
        public KeyCode RotateRight;
        public KeyCode Shoot;

        public KeyMapping(int playerID)
        {
            switch (playerID)
            {
                case 1:
                    Forward = KeyCode.W;
                    Backward = KeyCode.S;
                    RotateLeft = KeyCode.A;
                    RotateRight = KeyCode.D;
                    Shoot = KeyCode.Space;
                    break;

                case 2:
                    Forward = KeyCode.UpArrow;
                    Backward = KeyCode.DownArrow;
                    RotateLeft = KeyCode.LeftArrow;
                    RotateRight = KeyCode.RightArrow;
                    Shoot = KeyCode.RightAlt;
                    break;

                // not implementing players 3 & 4 for this class - would want to implement with controllers
                case 3:
                case 4:
                    throw new NotImplementedException();

                default:
                    throw new IndexOutOfRangeException(string.Format("Max players allowed is 4: Recieved player ID {0}", playerID));
            }

            _hasBeenSet = true;
        }
    }
    
    private KeyMapping _keyMapping;

    protected override void Update()
    {
        if (!_keyMapping.HasBeenSet)
        {
            _keyMapping = new KeyMapping(Settings.ID);
        }

        HandleMovementInput();
        HandleRotationInput();
        HandleShootingInput();
    }

    // Checks and sends input for positional movement.
    protected virtual void HandleMovementInput()
    {
        if (Input.GetKey(_keyMapping.Forward))
        {
            MotorComponent.Move(Settings.MovementSettings.Forward);
        }
        else if (Input.GetKey(_keyMapping.Backward))
        {
            MotorComponent.Move(-Settings.MovementSettings.Backward);
        }
    }

    // Checks and sends input for rotational movement.
    protected virtual void HandleRotationInput()
    {
        if (Input.GetKey(_keyMapping.RotateLeft))
        {
            MotorComponent.Rotate(-Settings.MovementSettings.Rotation);
        }
        else if (Input.GetKey(_keyMapping.RotateRight))
        {
            MotorComponent.Rotate(Settings.MovementSettings.Rotation);
        }
    }

    // Handles all logic for shooting input
    protected virtual void HandleShootingInput()
    {
        if (Input.GetKeyDown(_keyMapping.Shoot))
        {
            if (CanShoot())
            {
                Shoot();
            }
        }
    }
}
