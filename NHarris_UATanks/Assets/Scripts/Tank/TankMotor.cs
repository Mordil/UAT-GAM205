using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TankMotor : MonoBehaviour
{
    [SerializeField]
    private Transform _bodyTransform;

    [SerializeField]
    private CharacterController _characterController;

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
    /// Rotates the tank by the speed provided.
    /// </summary>
    /// <param name="speed">The speed in angles per second to rotate by.</param>
    public void Rotate(float speed)
    {
        // Maintain current "up" position, to rotate horizontally.
        Vector3 rotateVector = Vector3.up * speed;
        rotateVector *= Time.deltaTime;

        // apply the rotation
        _bodyTransform.Rotate(rotateVector, Space.Self);
    }

    /// <summary>
    /// Rotates the tank towards the target by the speed provided.
    /// </summary>
    /// <param name="target">The target's transform</param>
    /// <param name="speed">The rotation speed in angles per second.</param>
    public void RotateTowards(Transform target, float speed)
    {
        // sanity check to avoid console errors
        if (target == null)
        {
            return;
        }

        Vector3 rotateVector = target.position - _bodyTransform.position;
        Quaternion rotationDirections = Quaternion.LookRotation(rotateVector);
        _bodyTransform.rotation = Quaternion.RotateTowards(_bodyTransform.rotation, rotationDirections, speed * Time.deltaTime);
    }
}
