using L4.Unity.Common;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Rigidbody), typeof(Collider), typeof(AudioSource))]
public class PowerupAgent: BaseScript
{
    [SerializeField]
    private Powerup _powerupData;
    public Powerup PowerupData { get { return _powerupData; } }
    
    [SerializeField]
    private Collider _collider;
    [SerializeField]
    private AudioSource _audioSource;
    [ReadOnly]
    [SerializeField]
    private GameObject _instance;

    protected override void Awake()
    {
        base.Awake();

        if (_powerupData.VisualPrefab == null &&
            _instance != null)
        {
            return;
        }

        _instance = Instantiate(_powerupData.VisualPrefab, this.transform) as GameObject;
        _instance.name = gameObject.name + "_Visual";
        _instance.transform.position = this.transform.position;
        _instance.transform.rotation = this.transform.rotation;
    }

    protected override void CheckDependencies()
    {
        base.CheckDependencies();

        this.CheckAndAssignIfDependencyIsNull(ref _collider, true);
        this.CheckAndAssignIfDependencyIsNull(ref _audioSource, true);
    }

    protected virtual void OnTriggerEnter(Collider otherObj)
    {
        _collider.enabled = false;
        _instance.SetActive(false);

        if (_powerupData != null && _powerupData.SoundEffect != null)
        {
            _audioSource.PlayOneShot(_powerupData.SoundEffect);

            Destroy(this.gameObject, _powerupData.SoundEffect.length);
        }
    }
}
