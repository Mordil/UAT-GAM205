using L4.Unity.Common;
using System;
using UnityEngine;

[AddComponentMenu("Powerups/Time Freeze")]
[RequireComponent(typeof(Rigidbody))]
public class PowerupTimeFreeze : PowerupBase
{
    public override bool IsPermanent { get { return _isPermanent; } }
    public override bool IsPickup { get { return false; } }
    public override bool HasExpired { get { return false; } }

    public override float Duration { get { return _duration; } }

    [SerializeField]
    private bool _isPermanent;
    [ReadOnly]
    [SerializeField]
    private bool _isActive;

    [SerializeField]
    private float _duration = 1.5f;
    [ReadOnly]
    [SerializeField]
    private float _timeRemaining;

    protected override void Update()
    {
        base.Update();

        if (_isActive && !_isPermanent)
        {
            _timeRemaining -= Time.deltaTime;

            if (_timeRemaining <= 0)
            {
                OnExpire(null);
            }
        }
    }

    public override void OnPickup(TankController controller)
    {
        GameManager.Instance.CurrentScene.As<MainLevel>().IsTimeFrozen = true;

        _isActive = true;
        _timeRemaining = _duration;
    }

    public override void OnExpire(TankController controller)
    {
        GameManager.Instance.CurrentScene.As<MainLevel>().IsTimeFrozen = false;

        base.OnExpire(controller);
    }
}
