using UnityEngine;
using System.Collections;
using L4.Unity.Common;
using System;

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
        if (HasDoorValue(doorsToClose, Doors.North)) { _doors.North.SetActive(false); }
        if (HasDoorValue(doorsToClose, Doors.South)) { _doors.South.SetActive(false); }
        if (HasDoorValue(doorsToClose, Doors.East)) { _doors.East.SetActive(false); }
        if (HasDoorValue(doorsToClose, Doors.West)) { _doors.West.SetActive(false); }
    }

    private bool HasDoorValue(Doors value, Doors valueToCheck)
    {
        return (value & valueToCheck) != 0;
    }
}
