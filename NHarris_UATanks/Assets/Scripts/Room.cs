using L4.Unity.Common;
using System;
using System.Linq;
using UnityEngine;

public class Room : BaseScript
{
    [Flags]
    public enum Doors { None = 0, North = 2, South = 4, East = 8, West = 16 }

    [Serializable]
    private class DoorsContainer
    {
        public GameObject North;
        public GameObject South;
        public GameObject East;
        public GameObject West;
    }

    [SerializeField]
    private DoorsContainer _doors;
    [SerializeField]
    private GameObject[] _playerSpawners;
    [SerializeField]
    private GameObject[] _enemySpawners;
    [SerializeField]
    private GameObject[] _powerupSpawners;

    protected override void Start()
    {
        base.Start();

        _playerSpawners = GetGameObjectsForType(SpawnerBase.SpawnerType.Player);
        _enemySpawners = GetGameObjectsForType(SpawnerBase.SpawnerType.Enemy);
        _powerupSpawners = GetGameObjectsForType(SpawnerBase.SpawnerType.Powerup);
    }

    protected override void Awake()
    {
        Start();
    }

    protected override void CheckDependencies()
    {
        base.CheckDependencies();

        this.CheckIfDependencyIsNull(_doors.North, false);
        this.CheckIfDependencyIsNull(_doors.South, false);
        this.CheckIfDependencyIsNull(_doors.East, false);
        this.CheckIfDependencyIsNull(_doors.West, false);
    }

    public void OpenDoors(Doors doorsToClose)
    {
        // flag checks for which doors to close in 1 call
        if (HasDoorValue(doorsToClose, Doors.North)) { _doors.North.SetActive(false); }
        if (HasDoorValue(doorsToClose, Doors.South)) { _doors.South.SetActive(false); }
        if (HasDoorValue(doorsToClose, Doors.East)) { _doors.East.SetActive(false); }
        if (HasDoorValue(doorsToClose, Doors.West)) { _doors.West.SetActive(false); }
    }

    public GameObject[] GetActiveSpawnersForType(SpawnerBase.SpawnerType type)
    {
        GameObject[] array;

        // determine which array to return based on the type parameter
        switch (type)
        {
            case SpawnerBase.SpawnerType.Enemy:
                array = _enemySpawners;
                break;

            case SpawnerBase.SpawnerType.Powerup:
                array = _powerupSpawners;
                break;

            case SpawnerBase.SpawnerType.Player:
            default:
                array = _playerSpawners;
                break;
        }

        // if they're disabled, they're no use to the caller
        // note: this is mostly dead code, as I was planning on implementing a "randomize configuration" feature where spawners will be enabled/disabled during map generation
        // but due to time, I cut that and all spawners are active all the time
        return array.Where(x => x.activeInHierarchy).ToArray();
    }

    private bool HasDoorValue(Doors value, Doors valueToCheck)
    {
        return (value & valueToCheck) != 0;
    }

    private GameObject[] GetGameObjectsForType(SpawnerBase.SpawnerType type)
    {
        return GetComponentsInChildren<SpawnerBase>()
            .Where(x => x.EntityType == type)
            .Select(x => x.gameObject)
            .ToArray();
    }
}
