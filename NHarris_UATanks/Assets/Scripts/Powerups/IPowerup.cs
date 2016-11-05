using L4.Unity.Common;
using UnityEngine;

// I'm not sure if an interface is that beneficial as I implement it with a base class... I was originally planning on each powerup implementing the interface
// if I can find time I'll rework it - but I doubt I will. This is how games have a bunch of "legacy" code in their shipped versions.
// "If it ain't addin' value, it ain't gettin' done."

public interface IPowerup
{
    bool IsPermanent { get; }
    bool IsPickup { get; }
    bool HasExpired { get; }

    float Duration { get; }

    void OnPickup(TankController controller);
    void OnUpdate(TankController controller);
    void OnExpire(TankController controller);
}

public abstract class PowerupBase : BaseScript, IPowerup
{
    public abstract bool IsPermanent { get; }
    /// <summary>
    /// If true, this object should be retained as a reference to call OnUpdate() every Update()
    /// </summary>
    public abstract bool IsPickup { get; }
    public abstract bool HasExpired { get; }

    public abstract float Duration { get; }

    public abstract void OnPickup(TankController controller);
    public virtual void OnUpdate(TankController controller) { }
    public virtual void OnExpire(TankController controller)
    {
        Destroy(gameObject);
    }

    [SerializeField]
    protected SphereCollider PickupCollider;
    [SerializeField]
    protected MeshRenderer PickupRenderer;
    [SerializeField]
    protected AudioSource _pickupAudioSource;

    protected override void Awake()
    {
        Start();
    }

    protected override void CheckDependencies()
    {
        base.CheckDependencies();

        this.CheckAndAssignIfDependencyIsNull(ref PickupCollider, true);
        this.CheckAndAssignIfDependencyIsNull(ref PickupRenderer, true);

        this.CheckIfDependencyIsNull(_pickupAudioSource);
    }

    protected virtual void OnTriggerEnter(Collider otherObj)
    {
        // hide the renderer and collider, as this object isn't destroyed until OnExpire() is called
        PickupCollider.enabled = false;
        PickupRenderer.enabled = false;
        _pickupAudioSource.Play();
    }
}
