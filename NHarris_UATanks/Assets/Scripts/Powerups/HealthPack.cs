using UnityEngine;

[CreateAssetMenu(fileName = "New Health Pack", menuName ="Powerup/Health Pack")]
public class HealthPack : Powerup
{
    // This expires as soon as it is picked up.
    public override bool HasExpired { get { return true; } }
    public override float Duration { get { return 0f; } }

    [SerializeField]
    private int _amount;

    public override void OnPickup(TankController controller)
    {
        if (IsPermanent)
        {
            controller.Settings.ModifyStat(health: _amount);
        }

        controller.AddHealth(_amount);
    }

    public override void OnExpire(TankController controller) { }

    public override void OnUpdate(TankController controller) { }
}
