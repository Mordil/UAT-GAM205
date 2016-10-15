using System;
using UnityEngine;

[RequireComponent(typeof (TankSettings), typeof(TankMotor), typeof(TankShooter))]
public class InputController : MonoBehaviour
{
    private DateTime _timeLastFired;
    [SerializeField]
    private TankSettings _settings;
    [SerializeField]
    private TankMotor _motor;
    [SerializeField]
    private TankShooter _shooter;

	private void Start()
    {
        if (_settings == null)
        {
            _settings = GetComponent<TankSettings>();
        }

        if (_motor == null)
        {
            _motor = GetComponent<TankMotor>();
        }

        if (_shooter == null)
        {
            _shooter = GetComponent<TankShooter>();
        }
	}
	
	private void Update()
    {
        handleMovementInput();
        handleRotationInput();
        handleShootingInput();
    }

    // Checks and sends input for positional movement.
    private void handleMovementInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            _motor.Move(_settings.MovementSettings.Forward);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            _motor.Move(-_settings.MovementSettings.Backward);
        }
    }

    // Checks and sends input for rotational movement.
    private void handleRotationInput()
    {
        if (Input.GetKey(KeyCode.A))
        {
            _motor.Rotate(-_settings.MovementSettings.Rotation);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            _motor.Rotate(_settings.MovementSettings.Rotation);
        }
    }

    // Handles all logic for shooting input
    private void handleShootingInput()
    {
        // get input
        // if input -> check if can fire
        // fire if can
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TimeSpan difference = DateTime.Now - _timeLastFired;

            if (difference.Seconds >= _settings.RateOfFire)
            {
                _shooter.Fire();
                _timeLastFired = DateTime.Now;
            }
        }
    }
}
