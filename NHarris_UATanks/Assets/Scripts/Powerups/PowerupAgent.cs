using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class PowerupAgent: MonoBehaviour
{
    [SerializeField]
    private Powerup _powerupData;
    public Powerup PowerupDate { get { return _powerupData; } }

    [ReadOnly]
    [SerializeField]
    private GameObject _instance;

    private void Awake()
    {
        if (_powerupData.VisualPrefab == null && _instance != null) return;

        _instance = Instantiate(_powerupData.VisualPrefab, this.transform) as GameObject;
        _instance.name = gameObject.name + "_Visual";
        _instance.transform.position = this.transform.position;
        _instance.transform.rotation = this.transform.rotation;
    }
}
