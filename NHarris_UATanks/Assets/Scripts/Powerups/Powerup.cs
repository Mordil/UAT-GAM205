using UnityEngine;

public abstract class Powerup : ScriptableObject
{
    public enum PossiblePickupEntities { Player, AI, Both }

    [SerializeField]
    private bool _isPermanent;
    public virtual bool IsPermanent { get { return _isPermanent; } }

    [SerializeField]
    [Tooltip("What entities are allowed to pickup this powerup?")]
    private PossiblePickupEntities _canBePickedUpBy;
    public PossiblePickupEntities CanBePickedUpBy { get { return _canBePickedUpBy; } }

    [SerializeField]
    private AudioClip _soundEffect;
    public AudioClip SoundEffect { get { return _soundEffect; } }

    [SerializeField]
    private GameObject _visualPrefab;
    public GameObject VisualPrefab { get { return _visualPrefab; } }
    
    public abstract bool HasExpired { get; }

    public abstract float Duration { get; }

    public abstract void OnPickup(TankController controller);
    public abstract void OnUpdate(TankController controller);
    public abstract void OnExpire(TankController controller);
}
