using UnityEngine;

[CreateAssetMenu(fileName = "New_TankDeathSettings", menuName = "Tank/Settings/Death")]
public class TankDeathSettings : ScriptableObject
{
    [SerializeField]
    private GameObject _deathVFXPrefab;
    public GameObject DeathVFXPrefab;
}
