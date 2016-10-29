using UnityEngine;
using System.Collections;
using L4.Unity.Common;

[AddComponentMenu("Powerups/Time Freeze")]
[RequireComponent(typeof(Rigidbody))]
public class PowerupTimeFreeze : BaseScript, IPowerup
{
    public bool IsPermanent { get { return _isPermanent; } }
    public bool IsPickup { get { return false; } }

    public float Duration { get { return _duration; } }

    [SerializeField]
    private bool _isPermanent;
    [ReadOnly]
    [SerializeField]
    private bool _isActive;

    [SerializeField]
    private float _duration = 1.5f;
    [ReadOnly]
    [SerializeField]
    private float _timeRemaining;

    [SerializeField]
    private SphereCollider _collider;
    [SerializeField]
    private MeshRenderer _renderer;

    protected override void Awake()
    {
        Start();
    }

    protected override void CheckDependencies()
    {
        base.CheckDependencies();

        this.CheckAndAssignIfDependencyIsNull(ref _collider, true);
        this.CheckAndAssignIfDependencyIsNull(ref _renderer, true);
    }

    protected override void Update()
    {
        if (_isActive)
        {
            _timeRemaining -= Time.deltaTime;

            if (_timeRemaining <= 0)
            {
                OnExpire(null);
            }
        }
	}
    
    private void OnTriggerEnter(Collider otherObj)
    {
        if (otherObj.gameObject.IsOnSameLayer(ProjectSettings.Layers.Player))
        {
            OnPickup(null);
        }
    }

    public void OnPickup(TankController controller)
    {
        GameManager.Instance.CurrentScene.As<MainLevel>().IsTimeFrozen = true;

        _isActive = true;
        _timeRemaining = _duration;
        _collider.enabled = false;
        _renderer.enabled = false;
    }

    public void OnUpdate(TankController controller) { }

    public void OnExpire(TankController controller)
    {
        GameManager.Instance.CurrentScene.As<MainLevel>().IsTimeFrozen = false;
        Destroy(gameObject);
    }
}
