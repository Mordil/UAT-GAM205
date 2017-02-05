using L4.Unity.Common;
using UnityEngine;

[CreateAssetMenu(fileName = "New Time Freeze", menuName = "Powerup/Time Freeze")]
public class TimeFreeze : Powerup
{
    [SerializeField]
    [Range(0.01f, 60)]
    private float _duration = 1.5f;
    public override float Duration { get { return _duration; } }
    
    public override bool HasExpired { get { return !IsPermanent && _timeRemaining <= 0; } }

    [ReadOnly]
    [SerializeField]
    private float _timeRemaining;

    public override void OnPickup(TankController controller)
    {
        // set the level as frozen
        GameManager.Instance.CurrentScene.As<MainLevel>().IsTimeFrozen = true;

        _timeRemaining = _duration;
    }

    public override void OnUpdate(TankController controller)
    {
        // only update time if it's not permanent
        if (IsPermanent)
        {
            _timeRemaining -= Time.deltaTime;
        }
    }

    public override void OnExpire(TankController controller)
    {
        // unfreeze the level and call the parent expiration implementation
        GameManager.Instance.CurrentScene.As<MainLevel>().IsTimeFrozen = false;
    }
}
