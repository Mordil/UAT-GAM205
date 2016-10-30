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

    [Header("Random Generation Settings")]
    public int MinRows = 2;
    public int MaxRows = 5;
    public int MinColumns = 2;
    public int MaxColumns = 5;

    [Header("Tile Settings")]
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
    private MainLevelGeneratorSettings _mapGenerationSettings;

    [ReadOnly]
    [SerializeField]
    private Room[,] _roomsGrid;

    protected override void Start()
    {
        base.Start();
        
        _mapGenerationSettings.MapSeed = (_isLevelOfDay) ? GetDateAsInt() : (int)DateTime.Now.Ticks;
        UnityEngine.Random.InitState(_mapGenerationSettings.MapSeed);

        GenerateMap();
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

    protected override void LateUpdate()
    {
        base.Update();
    }

    protected override void CheckDependencies()
    {
        base.CheckDependencies();

        this.CheckIfDependencyIsNull(_environmentContainer);
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

    private int GetDateAsInt()
    {
        var now = DateTime.Now;
        return now.Year + now.Month + now.Day;
    }
}
