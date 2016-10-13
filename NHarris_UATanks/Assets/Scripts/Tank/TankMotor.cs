using UnityEngine;

public class TankMotor : MonoBehaviour
{
    // references to the gameobject containers' transforms for the tank body segments
    [SerializeField]
    private Transform _bodyTransform;

    [SerializeField]
    private CharacterController _characterController;

	// Use this for initialization
	private void Start ()
    {
        if (_characterController == null)
        {
            _characterController = GetComponent<CharacterController>();
        }

        if (_bodyTransform == null)
        {
            _bodyTransform = GetComponent<Transform>();
        }
	}
    
    /// <summary>
    /// Moves the tank forward by the the speed provided.
    /// </summary>
    /// <param name="speed">Meters per second to move by.</param>
    public void Move(float speed)
    {
        // Move forward from the current rotation / position
        Vector3 speedVector = transform.forward * speed;

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
