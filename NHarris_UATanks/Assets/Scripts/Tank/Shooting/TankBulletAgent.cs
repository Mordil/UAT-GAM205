using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TankBulletAgent : MonoBehaviour
{
    public int Damage { get { return _bulletData.Damage; } }

    /// <summary>
    /// The tank that fired this bullet.
    /// </summary>
    public TankController Owner { get; private set; }

    [SerializeField]
    private TankBullet _bulletData;
    [SerializeField]
    private Rigidbody _rigidbody;

    private void OnCollisionEnter(Collision otherObj)
    {
        // if the collision is not with the shooter
        if (Owner != null && Owner.gameObject != otherObj.gameObject)
        {
            Die();
        }
    }

    /// <summary>
    /// Initializes the bullet with the settings provided.
    /// </summary>
    /// <param name="forwardVector">The forward vector of the shooter.</param>
    /// <param name="creatorLayer">The game object initializing the bullet.</param>
    /// <param name="bulletData">The bullet that is being fired</param>
    public void Initialize(Vector3 forwardVector, TankController owner, TankBullet bulletData)
    {
        Owner = owner;
        _bulletData = bulletData;

        // To translate forward movement to 1 meter per second (the rate _speed is at),
        // it needs to be multiplied by 50 units.
        _rigidbody.AddForce(forwardVector * (_bulletData.MovementSpeed * 50), ForceMode.Force);

        // Schedule this object to be destroyed
        StartCoroutine(WaitToDie(_bulletData.Lifespan));
    }

    private void Die()
    {
        // instantiate an explosion and schedule it's destruction
        var explosion = Instantiate(
            _bulletData.ExplosionVFXPrefab,
            transform.position,
            _bulletData.ExplosionVFXPrefab.transform.rotation) as GameObject;
        Destroy(explosion, _bulletData.ExplosionVFXDuration);

        // destroy this gameobject
        Destroy(this.gameObject);
    }

    private IEnumerator WaitToDie(float secondsToWait)
    {
        yield return new WaitForSeconds(secondsToWait);

        Die();
    }
}
