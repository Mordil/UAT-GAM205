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

    protected override void Awake()
    {
        Start();
    }

    protected override void Update()
    {
        base.Update();

        if (_shouldRotate)
        {
            _myTransform.Rotate(_rotationAngles * Time.deltaTime);
        }
    }

    protected override void CheckDependencies()
    {
        base.CheckDependencies();

        this.CheckAndAssignIfDependencyIsNull(ref _myTransform, true);
    }
}
