//using L4.Unity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Personality
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
    /// Patrol tank that phases between visible and invisible.
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
public class ShiftPhaserPersonalitySettings
{
    public OpacityPhaserSettings Settings;
}

[Serializable]
public class AITankSettings
{
    public float PatrolPointThreshold { get { return _patrolPointThreshold; } }
    /// <summary>
    /// PatrolPointThreshold squared.
    /// </summary>
    public float PatrolPointThresholdMagnitude { get { return _patrolPointThreshold * _patrolPointThreshold; } }

    public Personality SelectedPersonality { get { return _personality; } }
    public PatrolMode SelectedPatrolMode { get { return _patrolMode; } }
    public ShiftPhaserPersonalitySettings ShiftPhasingSettings { get { return _shiftPhasingSettings; } }

    [SerializeField]
    [Tooltip("The distance (in meters) the tank needs to reach towards a patrol point before selecting the next one.")]
    private float _patrolPointThreshold = 10f;
    [SerializeField]
    [Tooltip("The units the tank can hear the player (in meters).")]
    private float _hearingDistance = 20f;
    [SerializeField]
    [Tooltip("The angle (in degrees) the tank can see, with the halfway point being forward.")]
    private float _lineOfSightAngle = 120f;

    [SerializeField]
    private AggressivePersonalitySettings _agressiveSettings;
    [SerializeField]
    private GuardCaptainPersonalitySettings _guardCaptainSettings;
    [SerializeField]
    private ShiftPhaserPersonalitySettings _shiftPhasingSettings;
    [SerializeField]
    private Personality _personality = Personality.Standard;
    [SerializeField]
    private PatrolMode _patrolMode = PatrolMode.Loop;

    [SerializeField]
    [Tooltip("The places the tank will patrol between when in patrol mode.")]
    private List<Transform> _patrolPoints;
    public List<Transform> PatrolPoints { get { return _patrolPoints; } }
}

public class AIInputController : InputControllerBase
{
    // Which direction in the list is it navigating through
    private enum PatrolDirection { Forward, Backwards }

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

    public ActionMode CurrentActionMode { get { return _currentActionMode; } }

    private bool _hasSeenPlayerAlready;
    private bool _hasSeenPlayer;

    private int _currentPatrolPointIndex;
    
    private ActionMode _currentActionMode = ActionMode.Patrol;
    private PatrolDirection _patrolDirection = PatrolDirection.Forward;
    [SerializeField]
    private AITankSettings _aiSettings;

    protected override void Awake()
    {
        // because a tank can be instantiated at runtime, start won't be called, so we do it ourselves
        base.Start();

        // if the AI is meant to do phase shifting, check to see if it has the proper component
        if (_aiSettings.SelectedPersonality == Personality.PhaseShift)
        {
            // if not, add it and initialize its settings
            if (GetComponent<OpacityPhaser>() == null)
            {
                this.gameObject.AddComponent<OpacityPhaser>();
                GetComponent<OpacityPhaser>().Initialize(_aiSettings.ShiftPhasingSettings.Settings);
            }
        }
    }

    protected override void Update()
    {
        switch (_currentActionMode)
        {
            case ActionMode.Patrol:
            default:
                if (_aiSettings.PatrolPoints.Count > 0)
                {
                    PatrolUpdate();
                }
                break;
        }
    }

    private void PatrolUpdate()
    {
        Transform currentPoint = _aiSettings.PatrolPoints[_currentPatrolPointIndex];

        if (!IsLookingAtPatrolPoint(currentPoint))
        {
            MotorComponent.RotateTowards(currentPoint, Settings.MovementSettings.Rotation);
        }
        else
        {
            MotorComponent.Move(Settings.MovementSettings.Forward);
        }

        PatrolPointUpdate(currentPoint);
    }

    private void PatrolPointUpdate(Transform currentPoint)
    {
        // if the tank is too far from the patrol point, the next one shouldn't be selected
        if (GetDistanceFromObject(currentPoint) > _aiSettings.PatrolPointThresholdMagnitude)
        {
            return;
        }

        // otherwise, we need to select the next point based on the patrol mode
        switch (_aiSettings.SelectedPatrolMode)
        {
            case PatrolMode.Sequence:
                // if the tank is navigating forward through the list 0...10
                if (_patrolDirection == PatrolDirection.Forward)
                {
                    // check if the index is not the last, if true just increment
                    if (_currentPatrolPointIndex < _aiSettings.PatrolPoints.Count - 1)
                    {
                        _currentPatrolPointIndex++;
                    }
                    else
                    {
                        // otherwise change the direction for the next loop
                        _patrolDirection = PatrolDirection.Backwards;
                    }
                }
                else
                {
                    // check to see if the index is not the first, if true just decrement
                    if (_currentPatrolPointIndex > 0)
                    {
                        _currentPatrolPointIndex--;
                    }
                    else
                    {
                        // otherwise change direction for the next loop
                        _patrolDirection = PatrolDirection.Forward;
                    }
                }
                break;

            case PatrolMode.Loop:
            default:
                // if the current index is the last
                if (_currentPatrolPointIndex == _aiSettings.PatrolPoints.Count - 1)
                {
                    // reset the index to the first
                    _currentPatrolPointIndex = 0;
                }
                else
                {
                    // otherwise just increment
                    _currentPatrolPointIndex++;
                }
                break;
        }
    }

    private bool IsLookingAtPatrolPoint(Transform pointToCheck)
    {
        Quaternion directions = Quaternion.LookRotation(pointToCheck.position - MyTransform.position);

        return directions == MyTransform.rotation;
    }
}
