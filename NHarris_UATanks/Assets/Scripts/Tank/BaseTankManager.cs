using UnityEngine;

public abstract class BaseTankManager : MonoBehaviour
{
    public const string KILLED_TARGET_MESSAGE = "OnKilledTarget";

    public int ID { get; set; }

    [SerializeField]
    protected TankSettings Settings;

    protected virtual void Awake()
    {
        foreach (var component in GetComponents<ITankComponent>())
        {
            component.SetUp(Settings);
        }
    }

    public virtual void OnKilled()
    {
        Die();
    }

    protected virtual void Die()
    {
        var deathPrefab = Instantiate(
            Settings.DeathSettings.DeathVFXPrefab,
            this.gameObject.transform.position,
            Settings.DeathSettings.DeathVFXPrefab.transform.rotation) as GameObject;

        Destroy(deathPrefab, deathPrefab.GetComponent<ParticleSystem>().main.duration);
        Destroy(this.gameObject);
    }
}
