using UnityEngine;

[CreateAssetMenu(fileName = "New Triple Shot", menuName = "Powerup/Triple Shot")]
public class TripleShot : Powerup
{
    [SerializeField]
    private float _duration = 1.5f;
    public override float Duration { get { return _duration; } }

    public override bool HasExpired { get { return _timeRemaining <= 0; } }

    [ReadOnly]
    [SerializeField]
    private float _timeRemaining;
    
    public override void OnPickup(TankController controller)
    {
        _timeRemaining = _duration;
    }

    public override void OnUpdate(TankController controller)
    {
        if (!IsPermanent)
        {
            _timeRemaining -= Time.deltaTime;
        }
    }

    public override void OnExpire(TankController controller) { }
}
