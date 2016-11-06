using L4.Unity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class MainLevelGeneratorSettings
{
    public bool RandomSize;

    public int MapSeed;

    public int Rows;
    public int Columns;
    
    public int MinRows = 2;
    public int MaxRows = 5;
    public int MinColumns = 2;
    public int MaxColumns = 5;
    
    public float tileWidth = 50f;
    public float tileHeight = 50f;

    public GameObject[] TilePrefabs;

    public GameObject GetRandomPrefab()
    {
        if (TilePrefabs.Length == 0)
        {
            throw new UnityException("No prefabs have been assigned to the MainLevel for map generation!");
        }

        return TilePrefabs[UnityEngine.Random.Range(0, TilePrefabs.Length - 1)];
    }
}

public class MainLevel : SceneBase
{
    public const string PLAYER_DIED_MESSAGE = "OnPlayerDeath";
    public const string ENEMY_SPAWNED_MESSAGE = "OnEnemySpawned";

    [SerializeField]
    private bool _isTimeFrozen;
    public bool IsTimeFrozen
    {
        get { return _isTimeFrozen; }
        set { _isTimeFrozen = value; }
    }

    [SerializeField]
    private List<GameObject> _playersList = new List<GameObject>();
    public List<GameObject> PlayersList { get { return _playersList; } }

    private List<GameObject> _enemyList = new List<GameObject>();
    public List<GameObject> EnemyList { get { return _enemyList; } }

    [SerializeField]
    private bool _isLevelOfDay;

    [SerializeField]
    private GameObject _environmentContainer;
    [SerializeField]
    private GameObject _playersContainer;
    [SerializeField]
    private GameObject _enemiesContainer;
    [SerializeField]
    private GameObject _playerPrefab;

    [SerializeField]
    private MainLevelGeneratorSettings _mapGenerationSettings;

    [ReadOnly]
    [SerializeField]
    private Room[,] _roomsGrid;

    [ReadOnly]
    [SerializeField]
    private List<GameObject> _playerSpawners;

    /// <summary>
    /// Key = Player ID
    /// Value = Player's lives remaining
    /// </summary>
    [ReadOnly]
    [SerializeField]
    private Dictionary<int, int> _playerLivesTable;

    protected override void Start()
    {
        base.Start();
        
        _mapGenerationSettings.MapSeed = (_isLevelOfDay) ? GetDateAsInt() : (int)DateTime.Now.Ticks;
        UnityEngine.Random.InitState(_mapGenerationSettings.MapSeed);

        _playerSpawners = new List<GameObject>();
        _playerLivesTable = new Dictionary<int, int>();

        GenerateMap();
        
        for (int i = 1; i <= GameManager.Instance.Settings.NumberOfPlayers; i++)
        {
            SpawnPlayer(i);
            _playerLivesTable.Add(i, GameManager.Instance.Settings.NumberOfLives);
        }
    }

    protected override void Awake()
    {
        base.Awake();

        // Grab all of the enemies at the start of the game to reference later
        // if dynamic spawning is added to the game, this will need to be updated
        _enemyList = _environmentContainer.GetComponentsInChildren<TankSettings>()
            .Where(x => x.IsPlayer == false)
            .Select(x => x.gameObject)
            .ToList();

        // Grab all of the players, which are child objects of the main level (or should be)
        _playersList = this.GetComponentsInChildren<TankSettings>()
            .Where(x => x.IsPlayer)
            .Select(x => x.gameObject)
            .ToList();
    }

    protected override void Update()
    {
        int playersRemaining = 0;
        foreach(var player in _playerLivesTable.Where(x => x.Value >= 0).Select(x => x.Key).ToList())
        {
            playersRemaining++;
        }

        if (playersRemaining == 0)
        {
            // TODO: Game over
            Debug.Log("GAME OVER");
        }
    }

    protected override void CheckDependencies()
    {
        base.CheckDependencies();

        this.CheckIfDependencyIsNull(_environmentContainer);
    }

    public void SpawnPlayer(int id, bool playerLostGame = false)
    {
        var player = Instantiate(_playerPrefab, GetRandomPlayerSpawner().position, _playerPrefab.transform.rotation) as GameObject;
        player.name = "Player_" + id;
        player.transform.SetParent(_playersContainer.transform);
        player.GetComponent<TankSettings>().ID = id;

        float cameraRectWidth = 1f;
        float cameraRectHeight = 1f;
        float cameraX = 0;
        float cameraY = 0;
        int numberOfPlayers = GameManager.Instance.Settings.NumberOfPlayers;

        switch (id)
        {
            case 1:
                if (numberOfPlayers > 1)
                {
                    cameraY = .5f;
                    cameraRectHeight = .5f;

                    if (numberOfPlayers == 4)
                    {
                        cameraRectWidth = .5f;
                    }
                }
                break;

            case 2:
                cameraRectHeight = .5f;

                if (numberOfPlayers > 2)
                {
                    cameraRectWidth = .5f;
                }
                break;

            case 3:
            case 4:
                cameraRectHeight = .5f;
                cameraRectWidth = .5f;
                cameraX = .5f;

                if (id == 4)
                {
                    cameraY = .5f;
                }
                break;

            default:
                throw new IndexOutOfRangeException();
        }

        var camera = player.GetComponentInChildren<Camera>();
        camera.rect = new Rect(cameraX, cameraY, cameraRectWidth, cameraRectHeight);

        if (playerLostGame)
        {
            player.gameObject.SetActive(false);
            camera.transform.SetParent(_playersContainer.transform);
            camera.gameObject.name = "Player_" + id + "_DeathCamera";
        }
    }

    private void GenerateMap()
    {
        int rowsCount = (_mapGenerationSettings.RandomSize)
            ? UnityEngine.Random.Range(_mapGenerationSettings.MinRows, _mapGenerationSettings.MaxRows)
            : _mapGenerationSettings.Rows;
        int columnsCount = (_mapGenerationSettings.RandomSize)
            ? UnityEngine.Random.Range(_mapGenerationSettings.MinColumns, _mapGenerationSettings.MaxColumns)
            : _mapGenerationSettings.Columns;

        // sync the data so the inspector has correct info of the number of columns/rows if it was randomly generated
        _mapGenerationSettings.Rows = rowsCount;
        _mapGenerationSettings.Columns = columnsCount;

        _roomsGrid = new Room[columnsCount, rowsCount];

        for (int i = 0; i < rowsCount; i++)
        {
            for (int j = 0; j < columnsCount; j++)
            {
                float x = _mapGenerationSettings.tileWidth * j;
                float z = _mapGenerationSettings.tileHeight * i;
                var position = new Vector3(x, 0, z);

                GameObject newRoom = Instantiate(_mapGenerationSettings.GetRandomPrefab(), position, Quaternion.identity) as GameObject;
                newRoom.transform.SetParent(_environmentContainer.transform);
                newRoom.name = "Room_" + j + "_" + i;

                var roomScript = newRoom.GetComponent<Room>();
                _roomsGrid[j, i] = roomScript;

                OpenRoomDoors(roomScript, j, i, columnsCount, rowsCount);

                _playerSpawners = _playerSpawners
                    .Union(roomScript.GetActiveSpawnersForType(SpawnerBase.SpawnerType.Player))
                    .ToList();
            }
        }
    }

    private void OpenRoomDoors(Room roomToOpen, int x, int y, int maxColumns, int maxRows)
    {
        Room.Doors doorsToOpen = Room.Doors.None;

        if (y == 0)
        {
            doorsToOpen |= Room.Doors.North;
        }
        else if (y == maxRows - 1)
        {
            doorsToOpen |= Room.Doors.South;
        }
        else
        {
            doorsToOpen |= Room.Doors.North;
            doorsToOpen |= Room.Doors.South;
        }

        if (x == 0)
        {
            doorsToOpen |= Room.Doors.East;
        }
        else if (x == maxColumns - 1)
        {
            doorsToOpen |= Room.Doors.West;
        }
        else
        {
            doorsToOpen |= Room.Doors.East;
            doorsToOpen |= Room.Doors.West;
        }

        roomToOpen.OpenDoors(doorsToOpen);
    }

    private void OnPlayerDeath(int id)
    {
        int livesRemaining = _playerLivesTable[id]--;

        if (livesRemaining > 0)
        {
            SpawnPlayer(id);
        }
        else
        {
            SpawnPlayer(id, true);
        }
    }

    private void OnEnemySpawned(GameObject newEnemy)
    {
        newEnemy.transform.SetParent(_enemiesContainer.transform, true);
        _enemyList.Add(newEnemy);
    }

    private int GetDateAsInt()
    {
        var now = DateTime.Now;
        return now.Year + now.Month + now.Day;
    }

    private Transform GetRandomPlayerSpawner()
    {
        return _playerSpawners[UnityEngine.Random.Range(0, _playerSpawners.Count - 1)].transform;
    }
}
