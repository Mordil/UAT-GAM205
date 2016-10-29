using L4.Unity.Common;
using UnityEngine;

[AddComponentMenu("Animation/Effects/Rotation")]
public class Rotation : BaseScript
{
    [SerializeField]
    private bool _shouldRotate;

    [SerializeField]
    private Vector3 _rotationAngles;
    [SerializeField]
    private Transform _myTransform;

    protected override void Update()
    {
        base.Update();

        if (_shouldRotate)
        {
            transform.Rotate(_rotationAngles * Time.deltaTime);
        }
    }
}
