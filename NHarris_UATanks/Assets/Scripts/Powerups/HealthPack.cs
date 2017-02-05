using UnityEngine;

[CreateAssetMenu(fileName = "New Health Pack", menuName ="Powerup/Health Pack")]
public class HealthPack : Powerup
{
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
}
