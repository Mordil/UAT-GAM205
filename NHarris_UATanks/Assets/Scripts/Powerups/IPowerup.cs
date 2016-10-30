using L4.Unity.Common;
using System;
using UnityEngine;

[Flags]
public enum PowerupAffectedEntities { Players = 1, Enemies = 2 }

public static class PowerupAffectedEntitiesExtensions
{
    public static bool Is(this PowerupAffectedEntities self, PowerupAffectedEntities typeToCheck)
    {
        return (self & typeToCheck) == typeToCheck;
    }

    public static bool Has(this PowerupAffectedEntities self, PowerupAffectedEntities typeToCheck)
    {
        return (self | typeToCheck) == typeToCheck;
    }
}

public interface IPowerup
{
    bool IsPermanent { get; }
    bool IsPickup { get; }

    float Duration { get; }

    void OnPickup(TankController controller);
    void OnExpire(TankController controller);

    bool OnUpdate(TankController controller);
}

public static class IPowerupExtensions
{
    public static T CastAs<T>(this IPowerup self)
        where T : IPowerup
    {
        if (self is T)
        {
            return (T)(self);
        }

        return default(T);
    }
}

public abstract class PowerupBase : BaseScript, IPowerup
{
    public abstract bool IsPermanent { get; }
    public abstract bool IsPickup { get; }

    public abstract float Duration { get; }

    public abstract void OnPickup(TankController controller);
    public virtual void OnExpire(TankController controller)
    {
        Destroy(gameObject);
    }

    public virtual bool OnUpdate(TankController controller) { throw new NotImplementedException(); }

    [SerializeField]
    protected SphereCollider PickupCollider;
    [SerializeField]
    protected MeshRenderer PickupRenderer;

    protected override void Awake()
    {
        Start();
    }

    protected override void CheckDependencies()
    {
        base.CheckDependencies();

        this.CheckAndAssignIfDependencyIsNull(ref PickupCollider, true);
        this.CheckAndAssignIfDependencyIsNull(ref PickupRenderer, true);
    }

    protected virtual void OnTriggerEnter(Collider otherObj)
    {
        if (otherObj.gameObject.IsOnSameLayer(ProjectSettings.Layers.Player))
        {
            PickupCollider.enabled = false;
            PickupRenderer.enabled = false;
        }
    }
}
