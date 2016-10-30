using System;
using UnityEngine;

[AddComponentMenu("Powerups/Time Freeze")]
[RequireComponent(typeof(Rigidbody))]
public class PowerupHealthPack : PowerupBase
{
    public override bool IsPermanent { get { return _isPermanent; } }
    public override bool IsPickup { get { return false; } }
    public override bool HasExpired { get { return false; } }

    public override float Duration { get { return 0f; } }

    [SerializeField]
    [Tooltip("Add to the max health of the tank in addition to current health?")]
    private bool _isPermanent;

    [SerializeField]
    private int _value;

    public override void OnPickup(TankController controller)
    {
        if (_isPermanent)
        {
            controller.Settings.ModifyStat(health: _value);
        }

        controller.AddHealth(_value);

        OnExpire(null);
    }
}
