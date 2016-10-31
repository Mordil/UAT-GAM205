using UnityEngine;

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
        if (_nextSpawnTime == 0)
        {
            _nextSpawnTime = Time.time + _spawnDelay;
        }

        if (CanSpawn())
        {
            SpawnAITank();
        }
    }

    private void SpawnAITank()
    {
        GameObject tank = GetTankPrefabToSpawn();
        var spawnedInstance = Instantiate(tank, MyTransform.position + Vector3.up, tank.transform.rotation) as GameObject;

        _instancesSpawnedCount++;

        spawnedInstance.transform.SetParent(MyTransform, true);
        spawnedInstance.name = "AITank_" + spawnedInstance.GetHashCode();
        spawnedInstance.SetActive(true);

        _currentSpawnedInstance = spawnedInstance;
        _nextSpawnTime = 0;

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
