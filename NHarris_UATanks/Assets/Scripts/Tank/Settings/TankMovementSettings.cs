using UnityEngine;

[CreateAssetMenu(fileName = "New_TankMovementSettings", menuName = "Tank/Settings/Movement")]
public class TankMovementSettings : ScriptableObject
{
    [SerializeField]
    [Tooltip("Units (in meters) to move per second.")]
    private float _forward = 3f;
    public float Forward { get { return _forward; } }

    [SerializeField]
    [Tooltip("Units (in meters) to move per second.")]
    private float _backward = 3f;
    public float Backward { get { return _backward; } }

    [SerializeField]
    [Tooltip("Rotation in degrees the body should rotate.")]
    private float _rotation = 180f;
    public float Rotation { get { return _rotation; } }
}
