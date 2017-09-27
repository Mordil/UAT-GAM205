using L4.Unity.Common;
using UnityEngine;

[CreateAssetMenu(fileName = "New Time Freeze", menuName = "Powerup/Time Freeze")]
public class TimeFreeze : Powerup
{
    [SerializeField]
    [Range(0.01f, 60)]
    private float _duration = 1.5f;
    public override float Duration { get { return _duration; } }

    public override void OnPickup(GameObject entity)
    {
        // set the level as frozen
        GameManager.Instance.CurrentScene.As<MainLevel>().IsTimeFrozen = true;
    }

    public override void OnExpire(GameObject entity)
    {
        // unfreeze the level and call the parent expiration implementation
        GameManager.Instance.CurrentScene.As<MainLevel>().IsTimeFrozen = false;
    }
}
