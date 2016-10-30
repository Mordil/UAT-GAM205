using L4.Unity.Common;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Powerups/Spawner")]
public class PowerupSpawner : BaseScript
{
    private enum SpawnLogic { SinglePrefab, InSequence, Random }

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
    private Transform _myTransform;

    [SerializeField]
    private List<GameObject> _powerupPrefabs;

    protected override void Update()
    {
        if (!_spawnedInstanceCollider.enabled && _nextSpawnTime == 0)
        {
            _nextSpawnTime = Time.time + _spawnDelay;
        }

        if (CanSpawn())
        {
            GameObject powerup = GetPowerupToSpawn();
            SpawnPowerup(powerup);
        }
    }

    protected override void CheckDependencies()
    {
        this.CheckAndAssignIfDependencyIsNull(ref _myTransform, true);
    }

    private void SpawnPowerup(GameObject powerup)
    {
        var spawnedInstance = Instantiate(powerup, _myTransform.position + new Vector3(0, _heightOffset, 0), Quaternion.identity) as GameObject;

        _instancesSpawnedCount++;

        spawnedInstance.transform.SetParent(_myTransform, true);
        spawnedInstance.name = spawnedInstance.GetHashCode().ToString() + "_SpawnedPowerup_" + _instancesSpawnedCount.ToString();

        _spawnedInstanceCollider = spawnedInstance.GetComponent<SphereCollider>();
        _nextSpawnTime = 0;
    }

    private bool CanSpawn()
    {
        // if there is still an instance alive, return false
        if (_spawnedInstanceCollider.enabled || _powerupPrefabs.Count == 0)
        {
            return false;
        }

        bool isSpawnCountLowEnough = (_maxTimesToSpawn != 0) ?_maxTimesToSpawn < _instancesSpawnedCount : true;
        bool enoughTimeHasPassed = Time.time >= _nextSpawnTime;

        return isSpawnCountLowEnough && enoughTimeHasPassed;
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
