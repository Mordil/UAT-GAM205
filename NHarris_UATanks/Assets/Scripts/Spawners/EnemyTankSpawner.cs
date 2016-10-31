using UnityEngine;

// TODO: Refactor this into a base class for this & powerup spawner

[AddComponentMenu("Spawners/Enemy Tank Spawner")]
public class EnemyTankSpawner : SpawnerBase
{
    private enum SpawnLogic { SinglePrefab, InSequence, Random }

    public override SpawnerType EntityType { get { return SpawnerType.Enemy; } }

    [SerializeField]
    private int _maxToSpawn = 1;
    [ReadOnly]
    [SerializeField]
    private int _instancesSpawnedCount;
    [ReadOnly]
    [SerializeField]
    private int _spawnPrefabIndex;
    [SerializeField]
    private float _spawnDelay = 5f;
    [ReadOnly]
    [SerializeField]
    private float _nextSpawnTime;

    [SerializeField]
    [Tooltip("How does the spawner handle determining which prefab to use?")]
    private SpawnLogic _spawnLogic;

    [ReadOnly]
    [SerializeField]
    private GameObject _currentSpawnedInstance;

    [SerializeField]
    [Tooltip("The Enemy tank prefabs to spawn with.")]
    private GameObject[] _tankPrefabs;

    protected override void Start()
    {
        base.Start();

        SpawnAITank();
    }

    protected override void Update()
    {
        // if the time hasn't been set
        if (_nextSpawnTime == 0)
        {
            _nextSpawnTime = Time.time + _spawnDelay;
        }

        // if we can spawn, do it
        if (CanSpawn())
        {
            SpawnAITank();
        }
    }

    private void SpawnAITank()
    {
        // get the next prefab to spawn and instantiate it just above the spawner's transform, with the prefab's original rotation
        GameObject tank = GetTankPrefabToSpawn();
        var spawnedInstance = Instantiate(tank, MyTransform.position + Vector3.up, tank.transform.rotation) as GameObject;

        // increment the count for future spawning logic
        _instancesSpawnedCount++;

        // give a meaningful name, set it active (if the prefab was inactive for prefab limitation workarounds) and assign the parent for clean heirarchy
        spawnedInstance.transform.SetParent(MyTransform, true);
        spawnedInstance.name = "AITank_" + spawnedInstance.GetHashCode();
        spawnedInstance.SetActive(true);

        // store the reference and reset the timer
        _currentSpawnedInstance = spawnedInstance;
        _nextSpawnTime = 0;

        // send this message with the gameobject so the level can do necessary AITank spawning logic
        this.SendMessageUpwards(MainLevel.ENEMY_SPAWNED_MESSAGE, spawnedInstance);
    }

    private bool CanSpawn()
    {
        // if there is still an instance alive, return false
        if (_tankPrefabs.Length == 0 || _currentSpawnedInstance != null)
        {
            return false;
        }

        bool isSpawnCountLowEnough = (_maxToSpawn != 0) ? _instancesSpawnedCount < _maxToSpawn : true;
        bool enoughTimeHasPassed = Time.time >= _nextSpawnTime;

        return isSpawnCountLowEnough && enoughTimeHasPassed;
    }

    private GameObject GetTankPrefabToSpawn()
    {
        switch (_spawnLogic)
        {
            case SpawnLogic.InSequence:
                return GetNextTankPrefab();

            case SpawnLogic.Random:
                return _tankPrefabs[UnityEngine.Random.Range(0, _tankPrefabs.Length - 1)];

            case SpawnLogic.SinglePrefab:
            default:
                return _tankPrefabs[0];
        }
    }

    private GameObject GetNextTankPrefab()
    {
        // if the index is the last index, reset to -1 so when we increment before accessing it's at 0 again
        if (_spawnPrefabIndex == _tankPrefabs.Length)
        {
            _spawnPrefabIndex = -1;
        }

        _spawnPrefabIndex++;
        return _tankPrefabs[_spawnPrefabIndex];
    }
}
