using UnityEngine;

public class TankManager : MonoBehaviour
{
    [SerializeField]
    private bool _isPlayer;
    public bool IsPlayer { get { return _isPlayer; } }

    public int ID { get; set; }

    [SerializeField]
    private TankSettings _settings;

    private void Awake()
    {
        // assign the default ID of the tank
        ID = this.gameObject.GetHashCode();
    }
}
