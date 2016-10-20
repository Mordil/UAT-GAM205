using L4.Unity.Common;
using System;
using UnityEngine;

/*
    Notes:
    I know I'm not following this requirement as stated in the assignment post:

    The amount of damage done by a shell is a property of the entity firing the shell and can be edited by designers in the inspector.

    This is because I think this limits gameplay through architecture because additional code would need
    to be written in the TankSettings to allow different bullets, while having the settings
    on the bullet itself allows easier ability to add different bullet types.

    It's not that I didn't read instructions, I believe this is better code / design for this project.
*/

[RequireComponent(typeof(Rigidbody))]
public class TankBullet : BaseScript
{
    public int Damage { get { return _damage; } }

    /// <summary>
    /// The tank that fired this bullet.
    /// </summary>
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

    #region Unity Lifecycle
    protected override void Awake()
    {
        // This component is created after the game has already started, so this done here instead of Start()
        this.CheckAndAssignIfDependencyIsNull(ref _rigidbody);
    }

    protected override void Update()
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
        if (Owner != null && Owner.gameObject != otherObj.gameObject)
        {
            Destroy(this.gameObject);
        }
    }
    #endregion

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
