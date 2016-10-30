using System;
using UnityEngine;

[AddComponentMenu("Powerups/Triple Shot")]
[RequireComponent(typeof(Rigidbody))]
public class PowerupTripleShot : PowerupBase
{
    public override bool IsPermanent { get { return _isPermanent; } }
    public override bool IsPickup { get { return true; } }
    public override bool HasExpired { get { return (_isActive) ? _timeRemaining <= 0 : false; } }

    public override float Duration { get { return _duration; } }

    [SerializeField]
    [Tooltip("Add to the max health of the tank in addition to current health?")]
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
        }
    }

    public override void OnPickup(TankController controller)
    {
        _isActive = true;
        _timeRemaining = _duration;
    }
}
