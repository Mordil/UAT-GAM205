using L4.Unity.Common;
using UnityEngine;

public class TankMotor : BaseScript
{
    /// <summary>
    /// The Tank GameObject's current forward position.
    /// </summary>
    public Vector3 ForwardVector { get { return _bodyTransform.forward; } }

    // references to the gameobject containers' transforms for the tank body segments
    [SerializeField]
    private Transform _bodyTransform;

    [SerializeField]
    private CharacterController _characterController;

    protected override void CheckDependencies()
    {
        base.CheckDependencies();

        this.CheckAndAssignIfDependencyIsNull(ref _bodyTransform, true);
        this.CheckAndAssignIfDependencyIsNull(ref _characterController);
    }

    /// <summary>
    /// Moves the tank forward by the the speed provided.
    /// </summary>
    /// <param name="speed">Meters per second to move by.</param>
    public void Move(float speed)
    {
        // Move forward from the current rotation / position
        Vector3 speedVector = _bodyTransform.forward * speed;

        // simpleMove applies Time.deltaTime, so it's unneeded by us <-- DON'T FORGET
        _characterController.SimpleMove(speedVector);
    }

    /// <summary>
    /// Rotates a the tank by the speed provided.
    /// </summary>
    /// <param name="speed">The speed in meters per second to rotate by.</param>
    public void Rotate(float speed)
    {
        // Maintain current "up" position, to rotate horizontally.
        Vector3 rotateVector = Vector3.up * speed;
        rotateVector *= Time.deltaTime;

        // apply the rotation
        _bodyTransform.Rotate(rotateVector, Space.Self);
    }
}
