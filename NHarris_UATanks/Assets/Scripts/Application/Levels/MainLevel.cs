using L4.Unity.Common;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
    private bool _displayTestLogs = true;

    [SerializeField]
    private GameObject _enemyCollection;

    protected override void Start()
    {
        base.Start();

        // Grab all of the enemies at the start of the game to reference later
        // if dynamic spawning is added to the game, this will need to be updated
        _enemyList = _enemyCollection.GetComponentsInChildren<TankSettings>()
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

        // Stub to show the count is updating
        if (_enemyCollection != null && _displayTestLogs)
        {
            Debug.Log(string.Format("Enemies Alive: {0}", getEnemiesAliveCount()));
        }
    }

    protected override void CheckDependencies()
    {
        base.CheckDependencies();

        this.CheckIfDependencyIsNull(_enemyCollection);
    }

    private int getEnemiesAliveCount()
    {
        // Items in the list can be null and List.Count will report incorrect numbers.
        return _enemyList
                .Where(x => x != null)
                .ToList()
                .Count;
    }
}
