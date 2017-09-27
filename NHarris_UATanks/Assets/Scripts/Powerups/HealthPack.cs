using UnityEngine;

[CreateAssetMenu(fileName = "New Health Pack", menuName ="Powerup/Health Pack")]
public class HealthPack : Powerup
{
    public override float Duration { get { return 0f; } }

    [SerializeField]
    private int _amount;

    public override void OnPickup(GameObject entity)
    {
        var agent = entity.GetComponent<HealthAgent>();
        if (agent)
        {
            if (IsPermanent)
            {
                agent.ModifyMaxHealth(_amount);
            }

            agent.AddHealth(_amount);
        }
    }
}
