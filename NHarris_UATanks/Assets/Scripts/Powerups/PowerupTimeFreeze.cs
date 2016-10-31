using L4.Unity.Common;
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

        // only update time if it's currently active and not permanent
        if (_isActive && !_isPermanent)
        {
            _timeRemaining -= Time.deltaTime;

            // if no time is left, kill self
            // because this isn't a pickup, no other object should be calling this.
            if (_timeRemaining <= 0)
            {
                OnExpire(null);
            }
        }
    }

    public override void OnPickup(TankController controller)
    {
        // set the level as frozen
        GameManager.Instance.CurrentScene.As<MainLevel>().IsTimeFrozen = true;

        // set active and time for lifecycle tracking
        _isActive = true;
        _timeRemaining = _duration;
    }

    public override void OnExpire(TankController controller)
    {
        // unfreeze the level and call the parent expiration implementation
        GameManager.Instance.CurrentScene.As<MainLevel>().IsTimeFrozen = false;

        base.OnExpire(controller);
    }
}
