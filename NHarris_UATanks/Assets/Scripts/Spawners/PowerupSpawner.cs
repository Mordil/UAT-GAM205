﻿using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Spawners/Powerups Spawner")]
public class PowerupSpawner : SpawnerBase
{
    // How should picking prefabs work?
    private enum SpawnLogic { SinglePrefab, InSequence, Random }

    public override SpawnerType EntityType { get { return SpawnerType.Powerup; } }

    [SerializeField]
    [Tooltip("The number of instances this spawner will spawn before stopping. If 0, will spawn infinitely.")]
    private int _maxTimesToSpawn = 0;
    [ReadOnly]
    [SerializeField]
    private int _instancesSpawnedCount = 0;
    [ReadOnly]
    [SerializeField]
    private int _spawnPrefabIndex = 0;
    [SerializeField]
    [Tooltip("The delay (in seconds) between a powerup being picked up and the next instance being spawned.")]
    private float _spawnDelay = 1.5f;
    private float _nextSpawnTime = 0;
    [SerializeField]
    [Tooltip("The height above the spawner the spawned powerup should be at.")]
    private float _heightOffset = 1f;

    [SerializeField]
    [Tooltip("How does the spawner handle determining which prefab to use?")]
    private SpawnLogic _spawnLogic;

    [ReadOnly]
    [SerializeField]
    private SphereCollider _spawnedInstanceCollider;

    [SerializeField]
    private List<GameObject> _powerupPrefabs;

    protected override void Update()
    {
        // if the last spawned instance isn't "alive" and the time has been reset
        if (_nextSpawnTime == 0 && !CheckIsInstanceAlive())
        {
            // store the defined next spawn time
            _nextSpawnTime = Time.time + _spawnDelay;
        }

        if (CanSpawn())
        {
            // spawn the picked next powerup prefab
            GameObject powerup = GetPowerupToSpawn();
            SpawnPowerup(powerup);
        }
    }

    private void SpawnPowerup(GameObject powerup)
    {
        // spawn the prefab with the designer specified offset and its original rotation
        var spawnedInstance = Instantiate(powerup, MyTransform.position + new Vector3(0, _heightOffset, 0), powerup.transform.rotation) as GameObject;

        // increment the counter for CanSpawn() logic
        _instancesSpawnedCount++;

        // assign the parent for clean heirarchy and give it a meaningful name
        spawnedInstance.transform.SetParent(MyTransform, true);
        spawnedInstance.name = this.gameObject.GetHashCode().ToString() + "_SpawnedPowerup_" + _instancesSpawnedCount.ToString();

        // assign the reference for "is alive" logic and reset the timer
        _spawnedInstanceCollider = spawnedInstance.GetComponent<SphereCollider>();
        _nextSpawnTime = 0;
    }

    private bool CanSpawn()
    {
        // if there is still an instance alive, return false
        if (_powerupPrefabs.Count == 0 || CheckIsInstanceAlive())
        {
            return false;
        }

        // if this shouldn't spawn infinitely, check to see if we've spawned the max allowed
        bool isSpawnCountLowEnough = (_maxTimesToSpawn != 0) ? _instancesSpawnedCount < _maxTimesToSpawn : true;
        bool enoughTimeHasPassed = Time.time >= _nextSpawnTime;

        return isSpawnCountLowEnough && enoughTimeHasPassed;
    }

    private bool CheckIsInstanceAlive()
    {
        // powerups don't destroy when they're collected, they deactivate their renderer and collider
        return _spawnedInstanceCollider != null && _spawnedInstanceCollider.enabled;
    }

    private GameObject GetPowerupToSpawn()
    {
        switch (_spawnLogic)
        {
            case SpawnLogic.InSequence:
                return GetNextSpawnPrefab();

            case SpawnLogic.Random:
                return _powerupPrefabs[UnityEngine.Random.Range(0, _powerupPrefabs.Count - 1)];

            case SpawnLogic.SinglePrefab:
            default:
                return _powerupPrefabs[0];
        }
    }

    private GameObject GetNextSpawnPrefab()
    {
        // if the index is the last index, reset to -1 so when we increment before accessing it's at 0 again
        if (_spawnPrefabIndex == _powerupPrefabs.Count)
        {
            _spawnPrefabIndex = -1;
        }

        _spawnPrefabIndex++;
        return _powerupPrefabs[_spawnPrefabIndex];
    }
}
