using L4.Unity.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

public interface IPowerup
{
    bool IsPermanent { get; }
    
    float Duration { get; }

    void OnPickup(TankController controller);
    void OnExpire(TankController controller);
}

[Serializable]
public class Powerup : IPowerup
{
    public bool IsPermanent { get { return false; } }

    public float Duration { get { return 0f; } }

    public void OnPickup(TankController controller)
    {

    }

    public void OnExpire(TankController controller)
    {

    }
}

public class PowerupSpawner : BaseScript
{
    public enum PowerupType
    {
        SpeedBoost
    }

    private enum SpawnLogic { SinglePrefab, InSequence, Random }

    public PowerupType Type { get { return _powerupType; } }

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
    private float _timeLastSpawned = 0;
    [SerializeField]
    [Tooltip("The height above the spawner the spawned powerup should be at.")]
    private float _heightOffset = 1f;

    [SerializeField]
    [Tooltip("How does the spawner handle determining which prefab to use?")]
    private SpawnLogic _spawnLogic;
    [SerializeField]
    private PowerupType _powerupType;

    [ReadOnly]
    [SerializeField]
    private GameObject _spawnedInstance;
    [SerializeField]
    private Transform _myTransform;

    [SerializeField]
    private List<GameObject> _powerupPrefabs;

    protected override void CheckDependencies()
    {
        this.CheckAndAssignIfDependencyIsNull(ref _myTransform, true);
    }

    protected override void Update()
    {
        if (CanSpawn())
        {
            GameObject powerup = GetPowerupToSpawn();
            SpawnPowerup(powerup);
        }
    }

    private void SpawnPowerup(GameObject powerup)
    {
        _spawnedInstance = Instantiate(powerup,
            _myTransform.position + new Vector3(0, _heightOffset, 0),
            Quaternion.identity) as GameObject;
        _spawnedInstance.transform.SetParent(_myTransform, true);
        _instancesSpawnedCount++;
        _timeLastSpawned = Time.time;
    }

    private bool CanSpawn()
    {
        // if there is still an instance alive, return false
        if (_spawnedInstance != null || _powerupPrefabs.Count == 0)
        {
            return false;
        }

        bool isSpawnCountLowEnough = (_maxTimesToSpawn != 0) ?_maxTimesToSpawn < _instancesSpawnedCount : true;
        bool enoughTimeHasPassed = (Time.time - _timeLastSpawned) >= _spawnDelay;

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
