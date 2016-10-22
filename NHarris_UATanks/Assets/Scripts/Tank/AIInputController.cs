//using L4.Unity.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AggressivePersonalitySettings
{
    [SerializeField]
    [Tooltip("Adds the speed increase value to the tank's speed.")]
    private bool _increaseSpeed;
    [SerializeField]
    [Tooltip("Adds the health increase value to the tank's max health and current health.")]
    private bool _increaseHealth;
    [SerializeField]
    [Tooltip("Adds the fire rate increase value to the tank's shooting delay (in seconds).")]
    private bool _increaseFireRate;

    [SerializeField]
    private float _speedIncreaseValue;
    [SerializeField]
    private float _healthIncreaseValue;
    [SerializeField]
    private float _fireRateIncrease;
}

[Serializable]
public class GuardCaptainPersonalitySettings
{
    [SerializeField]
    private int _numberOfTanksToSpawn = 5;

    [SerializeField]
    private List<GameObject> _tanksToSpawnPrefabs;
}

[Serializable]
public class AITankSettings
{
    [SerializeField]
    [Tooltip("The units the tank can hear the player (in meters).")]
    private float _hearingDistance = 20f;
    [SerializeField]
    [Tooltip("The angle (in degrees) the tank can see, with the halfway point being forward.")]
    private float _lineOfSightAngle = 120f;

    [SerializeField]
    private AggressivePersonalitySettings _agressivePersonalitySettings;
    [SerializeField]
    private GuardCaptainPersonalitySettings _guardCaptainPersonalitySettings;

    [SerializeField]
    [Tooltip("The places the tank will patrol between when in patrol mode.")]
    private List<Transform> _patrolPoints;
    public List<Transform> PatrolPoints { get { return _patrolPoints; } }
}

public class AIInputController : InputController
{
    public enum Personaltiy
    {
        /// <summary>
        /// Chases the player until killed.
        /// </summary>
        Aggressive,
        /// <summary>
        /// Spawns additional tanks when first spotting the player.
        /// </summary>
        GuardCaptain,
        /// <summary>
        /// Patrols and chases the player if seen for a short duration.
        /// </summary>
        Standard,
        /// <summary>
        /// Patrol tank that goes phases between visible and invisible.
        /// </summary>
        PhaseShift,
        /// <summary>
        /// Flees the fight if too much health is lost.
        /// </summary>
        FrenchTank
    }

    public enum ActionMode { Chase, Flee, Patrol }

    /// <summary>
    /// Represents the style of patrol movement the tank follows.
    /// </summary>
    public enum PatrolMode { Loop, Sequence }

    public bool HasSeenPlayer
    {
        get { return _hasSeenPlayer; }
        private set
        {
            _hasSeenPlayer = value;

            if (!_hasSeenPlayerAlready)
            {
                _hasSeenPlayerAlready = true;
            }
        }
    }

    public Personaltiy SelectedPersonality { get { return _selectedPersonality; } }
    public ActionMode CurrentActionMode { get { return _currentActionMode; } }

    private bool _hasSeenPlayerAlready;
    private bool _hasSeenPlayer;

    private int _currentPatrolPointIndex;

    [SerializeField]
    private Personaltiy _selectedPersonality = Personaltiy.Standard;
    [SerializeField]
    private ActionMode _currentActionMode = ActionMode.Patrol;
    [SerializeField]
    private PatrolMode _patrolMode = PatrolMode.Loop;
    [SerializeField]
    private AITankSettings _aiSettings;

    protected override void Update()
    {
        // TODO: Update Logic
    }

    protected override void CheckDependencies()
    {
        base.CheckDependencies();
    }

    protected override void HandleMovementInput()
    {
        // TODO: Add movement AI
    }

    protected override void HandleRotationInput()
    {
        // TODO: Add Rotation AI
    }

    protected override void HandleShootingInput()
    {
        if (CanShoot())
        {
            Shoot();
        }
    }
}
