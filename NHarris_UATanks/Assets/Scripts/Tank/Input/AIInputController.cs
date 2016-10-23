using L4.Unity.Common;
using System;
using System.Collections.Generic;
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
public enum PatrolMode { Loop, Sequence, NoRepeat }

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
    public int FleeHealthThreshold { get { return _fleeHealthThreshold; } }
    public float PatrolPointThreshold { get { return _patrolPointThreshold; } }
    /// <summary>
    /// PatrolPointThreshold squared.
    /// </summary>
    public float PatrolPointThresholdMagnitude { get { return _patrolPointThreshold * _patrolPointThreshold; } }
    public float DelayBetweenPatrolPoints { get { return _delayBetweenPatrolPoints; } }
    public float FleeDistance
    {
        get
        {
            // French Tanks flee twice as far as normal tanks.
            if (SelectedPersonality == Personality.FrenchTank)
            {
                return _fleeDistance * 2;
            }
            return _fleeDistance;
        }
    }
    public float FleeTime
    {
        get
        {
            // French Tanks flee for twice as long as normal tanks.
            if (SelectedPersonality == Personality.FrenchTank)
            {
                return _fleeTime * 2;
            }
            return _fleeTime;
        }
    }

    public Personality SelectedPersonality { get { return _personality; } }
    public PatrolMode SelectedPatrolMode { get { return _patrolMode; } }
    public ShiftPhaserPersonalitySettings ShiftPhasingSettings { get { return _shiftPhasingSettings; } }

    [SerializeField]
    [Tooltip("The health at which the tank will start fleeing.")]
    private int _fleeHealthThreshold = 10;
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
    [Tooltip("The time (in seconds) the tank will wait before selecting and patrolling to the next patrol point.")]
    private float _delayBetweenPatrolPoints = 0f;
    [SerializeField]
    [Tooltip("The distance (in meters) the tank will run away from the target when below certain health threshold.")]
    private float _fleeDistance = 10f;
    [SerializeField]
    [Tooltip("The time (in seconds) the tank will wait at the flee position after reaching it before going back to patrol mode.")]
    private float _fleeTime = 10f;

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

[RequireComponent(typeof(TankController))]
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
    private bool _didSetTimeReachedPatrolPoint;

    private int _currentPatrolPointIndex;
    private float _timeReachedPatrolPoint;
    
    private ActionMode _currentActionMode = ActionMode.Patrol;
    private PatrolDirection _patrolDirection = PatrolDirection.Forward;
    [SerializeField]
    private AITankSettings _aiSettings;
    [SerializeField]
    private Transform _currentTarget;
    private Transform _fleeTarget;
    [SerializeField]
    private TankController _controller;

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
        // TODO: Check for player sight/sound

        HealthUpdate();

        switch (_currentActionMode)
        {
            case ActionMode.Chase:
                ChaseTargetUpdate();
                break;

            case ActionMode.Flee:
                FleeTargetUpdate();
                break;

            case ActionMode.Patrol:
            default:
                if (_aiSettings.PatrolPoints.Count > 0)
                {
                    PatrolUpdate();
                }
                break;
        }
    }

    protected void OnDestroy()
    {
        // if this hasn't been destroyed through normal gameplay, clean up
        if (_fleeTarget != null)
        {
            Destroy(_fleeTarget.gameObject);
        }
    }

    protected override void CheckDependencies()
    {
        base.CheckDependencies();

        this.CheckAndAssignIfDependencyIsNull(ref _controller);
    }

    // Handler method for the TankController message broadcast when hit by a bullet
    public void OnTookDamage(TankController shooter)
    {
        Transform shooterTransform = shooter.transform;

        // if our current target is not the shooter, switch targets
        if (_currentTarget != shooterTransform)
        {
            _currentTarget = shooterTransform;
        }
    }

    private void HealthUpdate()
    {
        if (_currentActionMode != ActionMode.Flee &&
            _controller.CurrentHealth <= _aiSettings.FleeHealthThreshold)
        {
            // assign the new state
            _currentActionMode = ActionMode.Flee;

            // reset variables needed for the flee state
            _didSetTimeReachedPatrolPoint = false;
            _timeReachedPatrolPoint = 0;
        }
    }

    private void PatrolUpdate()
    {
        Transform currentPoint = _aiSettings.PatrolPoints[_currentPatrolPointIndex];

        // if the tank is within range of the target, switch to the next
        if (GetDistanceFromObject(currentPoint) <= _aiSettings.PatrolPointThresholdMagnitude)
        {
            // if we haven't set it yet, do so now for PatrolPointUpdate() delay logic
            if (!_didSetTimeReachedPatrolPoint)
            {
                _didSetTimeReachedPatrolPoint = true;
                _timeReachedPatrolPoint = Time.time;
            }

            // if enough time has passed after reaching the patrol point, update to the next
            if (Time.time - _timeReachedPatrolPoint >= _aiSettings.DelayBetweenPatrolPoints)
            {
                // reset the variables as we're about to change patrol points
                _didSetTimeReachedPatrolPoint = false;
                _timeReachedPatrolPoint = 0;

                PatrolPointUpdate(currentPoint);
            }
        }
        // otherwise we need to rotate/move towards the target
        else
        {
            if (!IsLookingAtPatrolPoint(currentPoint))
            {
                MotorComponent.RotateTowards(currentPoint, Settings.MovementSettings.Rotation);
            }
            else
            {
                MotorComponent.Move(Settings.MovementSettings.Forward);
            }
        }
    }

    private void PatrolPointUpdate(Transform currentPoint)
    {
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

            case PatrolMode.NoRepeat:
                // if the current index is the last one, just return - there are no updates to do
                if (_currentPatrolPointIndex == _aiSettings.PatrolPoints.Count - 1)
                {
                    return;
                }
                // otherwise, increment the index
                else
                {
                    _currentPatrolPointIndex++;
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

    private void ChaseTargetUpdate()
    {
        MotorComponent.RotateTowards(_currentTarget, Settings.MovementSettings.Forward);
        MotorComponent.Move(Settings.MovementSettings.Forward);

        if (CanShoot())
        {
            Shoot();
        }
    }

    private void FleeTargetUpdate()
    {
        // if the target hasn't been set and we're not keeping track of time that means we just hit the Flee mode
        if (_fleeTarget == null && _timeReachedPatrolPoint == 0)
        {
            // get the position of the "invisible target" for fleeing by inverting the vector from the target
            Vector3 vectorAwayFromTarget = (_currentTarget.position - MyTransform.position) * -1;
            // normalize the fector to work with
            vectorAwayFromTarget.Normalize();
            vectorAwayFromTarget *= _aiSettings.FleeDistance;

            // Add the current position and the calculated vector distance away to get the final world space position
            // and set it to the new gameobject's position
            _fleeTarget = new GameObject(this.gameObject.name + "_FleeTarget").transform;
            _fleeTarget.position = vectorAwayFromTarget + MyTransform.position;
        }

        // if the tank has reached its flee point
        if (GetDistanceFromObject(_fleeTarget) <= _aiSettings.PatrolPointThresholdMagnitude)
        {
            // we're reusing this variable
            // TODO: review if this should be changed later to avoid bugs...
            if (_timeReachedPatrolPoint == 0)
            {
                _timeReachedPatrolPoint = Time.time;
            }
            else
            {
                // if enough time has passed, go back to patrolling
                if (Time.time - _timeReachedPatrolPoint >= _aiSettings.FleeTime)
                {
                    // destroy the gameobject
                    Destroy(_fleeTarget.gameObject);

                    _currentActionMode = ActionMode.Patrol;

                    // reset this variable
                    _timeReachedPatrolPoint = 0;
                }
                else // regain health
                {
                    _controller.RegenerateHealth();
                }
            }
        }
        else // we need to move towards the flee target still
        {
            MotorComponent.RotateTowards(_fleeTarget, Settings.MovementSettings.Rotation);
            MotorComponent.Move(Settings.MovementSettings.Forward);
        }
    }

    private bool IsLookingAtPatrolPoint(Transform pointToCheck)
    {
        Quaternion directions = Quaternion.LookRotation(pointToCheck.position - MyTransform.position);

        return directions == MyTransform.rotation;
    }
}
