using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using PowerupSpawnerAgent = PowerupAgent;

namespace UATTanks.Tank.Components
{
    public class PowerupAgent : MonoBehaviour, ITankComponent
    {
        public bool HasTripleShot { get { return _currentPickups.Where(x => x.Key is TripleShot).Count() > 0; } }

        [SerializeField]
        [ReadOnly]
        private Dictionary<Powerup, float> _currentPickups;

        private void Awake()
        {
            _currentPickups = new Dictionary<Powerup, float>();
        }

        private void Update()
        {
            // if there are no pickups, nothing to do this frame update
            if (_currentPickups.Count == 0) return;

            // loop through each expired powerup and expire it
            _currentPickups
                .Where(x => x.Value <= 0)
                .Select(x => x.Key)
                .ToList()
                .ForEach(x =>
                {
                    x.OnExpire(this.gameObject);
                    _currentPickups.Remove(x);
                });

            // loop through the remaining powerups so that they can receive updates
            // and update their remaining time count
            foreach (Powerup powerup in _currentPickups.Keys)
            {
                _currentPickups[powerup] -= Time.deltaTime;
                powerup.OnUpdate(this.gameObject);
            }

        }

        private void OnTriggerEnter(Collider otherObj)
        {
            if (otherObj.gameObject.IsOnSameLayer(ProjectSettings.Layers.Powerup))
            {
                var agent = otherObj.gameObject.GetComponent<PowerupSpawnerAgent>();

                Powerup powerup = (agent != null) ? agent.PowerupData : null;
                if (powerup != null)
                {
                    powerup.OnPickup(this.gameObject);
                    _currentPickups.Add(powerup, powerup.Duration);
                }
            }
        }

        public void SetUp(TankSettings settings) { }
    }
}
