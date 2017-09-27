using L4.Unity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;



[RequireComponent(typeof(TankSettings))]
public class TankController : BaseScript
{
    public bool HasTripleShot { get { return _currentPickups.Where(x => x.Key is TripleShot).Count() > 0; } }

    public TankSettings Settings { get { return _settings; } }
    
    private float _timeOfLastHealthGain;

    [SerializeField]
    private TankSettings _settings;

    [ReadOnly]
    [SerializeField]
    private Dictionary<Powerup, float> _currentPickups;

    #region Unity Lifecycle
    protected override void Start()
    {
        base.Start();
        
        _currentPickups = new Dictionary<Powerup, float>();
	}

    protected override void Update()
    {
        UpdatePickups();
	}

    protected virtual void OnTriggerEnter(Collider otherObj)
    {
        if (otherObj.gameObject.IsOnSameLayer(ProjectSettings.Layers.Powerup))
        {
            PowerupAgent agent = otherObj.gameObject.GetComponent<PowerupAgent>();

            Powerup powerup = (agent != null) ? agent.PowerupData : null;
            if (powerup != null)
            {
                powerup.OnPickup(this);
                _currentPickups.Add(powerup, powerup.Duration);
            }
        }
    }
    #endregion

    public void RegenerateHealth()
    {
        if (_timeOfLastHealthGain == 0)
        {
            _timeOfLastHealthGain = Time.time;
            return;
        }

        float timeDiff = Time.time - _timeOfLastHealthGain;

        // if it has been at least 1 second
        if (timeDiff >= 1)
        {
            _timeOfLastHealthGain = Time.time;
            _currentHealth += (int)(_settings.HealthRegenRate * timeDiff);
            _currentHealth = Mathf.Clamp(_currentHealth, 0, _settings.MaxHealth);
        }
    }

    private void UpdatePickups()
    {
        if (_currentPickups.Count == 0) return;

        List<Powerup> itemsToRemove = new List<Powerup>();
        List<Powerup> currentPickups = _currentPickups.Keys.ToList();

        // loop through the powerups so that they can receive updates
        foreach (Powerup powerup in currentPickups)
        {
            float timeRemaining = _currentPickups[powerup];
            
            if (timeRemaining <= 0)
            {
                itemsToRemove.Add(powerup);
            }
            else
            {
                powerup.OnUpdate(this);            

                if (!powerup.IsPermanent)
                {
                    _currentPickups[powerup] = timeRemaining - Time.deltaTime;                
                }
            }
        }

        itemsToRemove.ForEach(powerup =>
        {
            powerup.OnExpire(this);
            _currentPickups.Remove(powerup);
        });
    }
}
