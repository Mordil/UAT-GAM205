using UnityEngine;

public class TankSettings : MonoBehaviour
{
    public float BodyRotationSpeed { get { return _bodyRotationSpeed; } }
    public float MovementSpeed { get { return _movementSpeed; } }

    [SerializeField]
    [Tooltip("Units (in meters) to move per second.")]
    private float _movementSpeed = 3f;

    [Tooltip("Rotation in degrees the body should rotate.")]
    [SerializeField]
    private float _bodyRotationSpeed = 180f;
}
