using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TankBullet : MonoBehaviour
{
    public int Damage { get { return _damage; } }

    public TankController Owner { get; private set; }

    [SerializeField]
    private int _damage = 10;
    [SerializeField]
    [Tooltip("How long the bullet exists before it will self destroy.")]
    private float _lifespan = 3f;
    [SerializeField]
    [Tooltip("Units (in meters) the bullet moves per second.")]
    private float _speed = 2f;

    private DateTime _timeOfDeath;

    [SerializeField]
    private Rigidbody _rigidbody;

    private void Awake()
    {
        // This component is created after the game has already started, so this done here instead of Start()

        if (_rigidbody == null)
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
    }

    private void Update()
    {
        // if the time of death is now or in the past
        if (DateTime.Now >= _timeOfDeath)
        {
            // destroy the gameobject
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider otherObj)
    {
        // if the collision is not with the shooter
        if (otherObj.gameObject != Owner.gameObject)
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Initializes the bullet with the settings provided.
    /// </summary>
    /// <param name="forwardVector">The forward vector of the shooter.</param>
    /// <param name="creatorLayer">The game object initializing the bullet.</param>
    public void Initialize(Vector3 forwardVector, TankController owner)
    {
        this.Owner = owner;
        this._timeOfDeath = DateTime.Now.AddSeconds(_lifespan);
        
        // To translate forward movement to 1 meter per second (the rate _speed is at),
        // it needs to be multiplied by 50 units.
        _rigidbody.AddForce(forwardVector * (_speed * 50), ForceMode.Force);
    }
}
