using System;
using UnityEngine;

public class TankBullet : MonoBehaviour
{
    public int Damage { get; private set; }

    private DateTime _timeOfDeath;

    private void Update()
    {
        // if the time of death is now or in the past
        if (DateTime.Now >= _timeOfDeath)
        {
            // destroy the gameobject
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Initializes the bullet with the settings provided.
    /// </summary>
    /// <param name="damage">The damage this bullet will deal if hit.</param>
    /// <param name="lifespan">The time from now this will self destroy.</param>
    public void Initialize(int damage, float lifespan)
    {
        this.Damage = damage;
        this._timeOfDeath = DateTime.Now.AddSeconds(lifespan);
    }
}
