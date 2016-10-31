using L4.Unity.Common;
using UnityEngine;

public abstract class SpawnerBase : BaseScript
{
    public enum SpawnerType { Player, Enemy, Powerup }

    public abstract SpawnerType EntityType { get; }
    
    [SerializeField]
    protected Transform MyTransform;

    protected override void CheckDependencies()
    {
        this.CheckAndAssignIfDependencyIsNull(ref MyTransform, true);
    }
}
