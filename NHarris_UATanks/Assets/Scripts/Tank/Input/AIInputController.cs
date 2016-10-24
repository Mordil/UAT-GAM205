using L4.Unity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ActionMode { Chase, Flee, Patrol }

/// <summary>
/// Represents the style of patrol movement the tank follows.
/// </summary>
public enum PatrolStyle { Loop, Sequence, NoRepeat }

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
public class AIPatrolManager
{
    // Which direction in the list is it navigating through
    private enum PatrolDirection { Forward, Backwards }

    public int NumberOfPatrolPoints { get { return _patrolPoints.Count; } }
    /// <summary>
    /// Returns the PatrolPointThreshold as a magnitude (squared) value.
    /// </summary>
    public float PatrolPointThreshold { get { return _patrolPointThreshold * _patrolPointThreshold; } }

    public Transform CurrentPatrolPoint { get { return _patrolPoints[_currentPatrolPointIndex]; } }
    
    [ReadOnly]
    [SerializeField]
    private int _currentPatrolPointIndex;
    private float _timeReachedPatrolPoint;
    [SerializeField]
    [Tooltip("The distance (in meters) the tank needs to reach towards a patrol point before selecting the next one.")]
    private float _patrolPointThreshold = 10f;
    [SerializeField]
    [Tooltip("The time (in seconds) the tank will wait before selecting and patrolling to the next patrol point.")]
    private float _delayBetweenPatrolPoints = 0f;

    [SerializeField]
    private PatrolStyle _patrolStyle = PatrolStyle.Loop;
    [ReadOnly]
    [SerializeField]
    private PatrolDirection _patrolDirection = PatrolDirection.Forward;

    [SerializeField]
    [Tooltip("The places the tank will patrol between when in patrol mode.")]
    private List<Transform> _patrolPoints;

    public void PatrolPointUpdate()
    {
        // if we haven't set it yet, do so now for PatrolPointUpdate() delay logic
        if (_timeReachedPatrolPoint == 0)
        {
            _timeReachedPatrolPoint = Time.time;
        }

        // if enough time has passed after reaching the patrol point, update to the next
        if (Time.time - _timeReachedPatrolPoint >= _delayBetweenPatrolPoints)
        {
            // reset the variable for later
            _timeReachedPatrolPoint = 0;

            SwitchPatrolPoints();
        }
    }

    private void SwitchPatrolPoints()
    {
        switch (_patrolStyle)
        {
            case PatrolStyle.Sequence:
                // if the tank is navigating forward through the list 0...10
                if (_patrolDirection == PatrolDirection.Forward)
                {
                    // check if the index is not the last, if true just increment
                    if (_currentPatrolPointIndex < _patrolPoints.Count - 1)
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

            case PatrolStyle.NoRepeat:
                // if the current index is the last one, just return - there are no updates to do
                if (_currentPatrolPointIndex == _patrolPoints.Count - 1)
                {
                    return;
                }
                // otherwise, increment the index
                else
                {
                    _currentPatrolPointIndex++;
                }
                break;

            case PatrolStyle.Loop:
            default:
                // if the current index is the last
                if (_currentPatrolPointIndex == _patrolPoints.Count - 1)
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
}

[Serializable]
public class AIFleeManager
{
    public int HealthThresholdToFlee { get { return _healthThresholdToFlee; } }

    public Transform Target { get { return _fleeTarget; } }

    [SerializeField]
    [Tooltip("The health at which the tank will start fleeing.")]
    private int _healthThresholdToFlee = 10;
    [SerializeField]
    [Tooltip("The distance (in meters) the tank will run away from the target when below certain health threshold.")]
    private float _fleeDistance = 10f;
    [SerializeField]
    [Tooltip("The time (in seconds) the tank will wait at the flee position after reaching it before going back to patrol mode.")]
    private float _fleeTime = 10f;
    private float _timeReachedTarget;

    [ReadOnly]
    [SerializeField]
    private Transform _fleeTarget;

    public void Setup(Vector3 targetToFleePosition, Vector3 currentPosition, AIInputController.Personality selectedPersonality)
    {
        // get the position of the "invisible target" for fleeing by inverting the vector from the target
        Vector3 vectorAwayFromTarget = (targetToFleePosition - currentPosition) * -1;
        // normalize the fector to work with
        vectorAwayFromTarget.Normalize();
        // increase the mangitude by the setting
        // french tanks go twice as far
        vectorAwayFromTarget *= (selectedPersonality == AIInputController.Personality.FrenchTank) ? _fleeDistance * 2 : _fleeDistance;

        // Add the current position and the calculated vector distance away to get the final world space position
        // and set it to the new gameobject's position
        _fleeTarget = new GameObject(GetHashCode() + "_FleeTarget").transform;
        _fleeTarget.position = vectorAwayFromTarget + currentPosition;
    }

    public void Reset()
    {
        // reset the time for us in HasFinishedFleeing()
        _timeReachedTarget = 0;
    }

    public bool HasFinishedFleeing()
    {
        // if the target has just been reached, assign the current time and return false as this is just being set
        if (_timeReachedTarget == 0)
        {
            _timeReachedTarget = Time.time;
            return false;
        }

        // if enough time has passed, destroy the object and return true as there is no more fleeing to do
        if (Time.time - _timeReachedTarget >= _fleeTime)
        {
            UnityEngine.Object.Destroy(_fleeTarget.gameObject);
            return true;
        }

        // otherwise, return false
        return false;
    }
}

[Serializable]
public class AIVisionSettings
{
    public float HearingDistance { get { return _hearingDistance; } }
    public float LineOfSightAngle { get { return _lineOfSightAngle; } }

    [SerializeField]
    [Tooltip("The distance (in meters) the tank can hear the player. This also determines how far the tank can see.")]
    private float _hearingDistance = 20f;
    [SerializeField]
    [Tooltip("The angle (in degrees) the tank can see, with the halfway point being forward.")]
    private float _lineOfSightAngle = 120f;
}

[HelpURL("Assets/Scripts/Tank/README.md")]
[RequireComponent(typeof(TankController))]
public class AIInputController : InputControllerBase
{
    public enum Personality
    {
        /// <summary>
        /// Chases the player until killed.
        /// </summary>
        Aggressive,
        /// <summary>
        /// Patrols and chases the player if seen for a short duration.
        /// </summary>
        Standard,
        /// <summary>
        /// Tank does not patrol.
        /// </summary>
        Stationary,
        /// <summary>
        /// Patrol tank that phases between visible and invisible.
        /// </summary>
        PhaseShift,
        /// <summary>
        /// Flees the fight if too much health is lost.
        /// </summary>
        FrenchTank
    }

    // Which stage of movement is happening
    private enum MovementMode { Normal, Pathfinding }

    private float _pathfindingExitTime;

    [Header("AI Settings")]
    #region serialized fields
    [SerializeField]
    private float _maxTimeDoingPathfinding = 3f;

    [ReadOnly]
    [SerializeField]
    private ActionMode _currentActionMode = ActionMode.Patrol;
    [ReadOnly]
    [SerializeField]
    private MovementMode _currentMovementMode = MovementMode.Normal;
    [SerializeField]
    private Personality _personality = Personality.Standard;

    [ReadOnly]
    [SerializeField]
    private Transform _currentTarget;

    [SerializeField]
    private TankController _controller;
    [SerializeField]
    private OpacityPhaserSettings _shiftPhasingSettings;

    [SerializeField]
    private AggressivePersonalitySettings _agressiveSettings;
    [SerializeField]
    private AIFleeManager _fleeSettings;
    [SerializeField]
    private AIPatrolManager _patrolSettings;
    [SerializeField]
    private AIVisionSettings _visionSettings;

    [SerializeField]
    [Tooltip("The trigger sphere collider used for hearing detection and FOV distance.")]
    private SphereCollider _triggerSphereCollider;
    #endregion

    #region Unity Methods
    protected override void Awake()
    {
        // because a tank can be instantiated at runtime, start won't be called, so we do it ourselves
        base.Start();

        DoPersonalitySetup();

        // if the hearing/vision trigger isn't selected, find the first collider component that is a trigger
        if (_triggerSphereCollider == null)
        {
            _triggerSphereCollider = GetComponents<SphereCollider>()
                .Where(x => x.isTrigger)
                .First();
        }

        // make sure the trigger's radius is the proper size
        _triggerSphereCollider.radius = _visionSettings.HearingDistance;
    }

    protected override void Update()
    {
        // aggressive tanks don't flee
        if (_personality != Personality.Aggressive)
        {
            CheckIfShouldFlee();
        }

        // do the current action mode's update cycle
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
                // stationary targets don't patrol
                if (_personality != Personality.Stationary)
                {
                    PatrolUpdate();
                }
                break;
        }
    }

    protected virtual void OnDestroy()
    {
        // if this hasn't been destroyed through normal gameplay, manually clean it up
        if (_fleeSettings.Target != null)
        {
            Destroy(_fleeSettings.Target.gameObject);
        }
    }

    protected virtual void OnTriggerEnter(Collider otherObj)
    {
        if (otherObj.gameObject.IsOnSameLayer(ProjectSettings.Layers.Player))
        {
            Transform possibleTarget = otherObj.gameObject.transform;

            if (IsTargetWithinView(possibleTarget))
            {
                _currentTarget = possibleTarget;
                GoToMode(ActionMode.Chase);
            }
        }
    }
    #endregion

    #region Parent methods
    protected override void CheckDependencies()
    {
        base.CheckDependencies();

        this.CheckAndAssignIfDependencyIsNull(ref _controller);
    }
    #endregion

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

    private void DoPersonalitySetup()
    {
        switch (_personality)
        {
            // if the AI is meant to do phase shifting, check to see if it has the proper component
            case Personality.PhaseShift:
                // if not, add it and initialize its settings
                if (GetComponent<OpacityPhaser>() == null)
                {
                    this.gameObject.AddComponent<OpacityPhaser>();
                    GetComponent<OpacityPhaser>().Initialize(_shiftPhasingSettings);
                }
                break;

            // if the tank is a french tank, update its scale for visual feedback
            case Personality.FrenchTank:
                Vector3 newScale = MyTransform.localScale * .5f;
                newScale.y = 1;
                MyTransform.localScale = newScale;
                break;

            default:
                break;
        }
    }

    private void CheckIfShouldFlee()
    {
        // if we're not already fleeing and our health is low enough, go to flee mode
        if (_currentActionMode != ActionMode.Flee &&
            _controller.CurrentHealth <= _fleeSettings.HealthThresholdToFlee)
        {
            GoToMode(ActionMode.Flee);
        }
    }

    private void PatrolUpdate()
    {
        // If there are no patrol points, there's nothing to do this update cycle
        if (_patrolSettings.NumberOfPatrolPoints == 0)
        {
            return;
        }

        Transform currentPoint = _patrolSettings.CurrentPatrolPoint;

        // if the tank is within range of the target, switch to the next
        if (GetDistanceFromObject(currentPoint) <= _patrolSettings.PatrolPointThreshold)
        {
            _patrolSettings.PatrolPointUpdate();
        }
        // otherwise we need to rotate/move towards the target
        else
        {
            if (_currentMovementMode == MovementMode.Normal)
            {
                if (!IsLookingAtPatrolPoint(currentPoint))
                {
                    MotorComponent.RotateTowards(currentPoint, Settings.MovementSettings.Rotation);
                }
                else
                {
                    if (CanMove(Settings.MovementSettings.Forward))
                    {
                        MotorComponent.Move(Settings.MovementSettings.Forward);
                    }
                    else
                    {
                        _currentMovementMode = MovementMode.Pathfinding;
                    }
                }
            }
            else
            {
                DoPathfinding();
            }
        }
    }

    private void ChaseTargetUpdate()
    {
        // if the target has become null, then return to normal patrol mode and end update
        if (_currentTarget == null)
        {
            GoToMode(ActionMode.Patrol);
            return;
        }

        if (_currentMovementMode == MovementMode.Normal)
        {
            if (CanMove(Settings.MovementSettings.Forward))
            {
                if (!IsLookingAtPatrolPoint(_currentTarget))
                {
                    MotorComponent.RotateTowards(_currentTarget, Settings.MovementSettings.Rotation);
                }

                if (GetDistanceFromObject(_currentTarget) >= _patrolSettings.PatrolPointThreshold)
                {
                    MotorComponent.Move(Settings.MovementSettings.Forward);
                }

                if (CanShoot())
                {
                    Shoot();
                }
            }
            else
            {
                _currentMovementMode = MovementMode.Pathfinding;
            }
        }
        else
        {
            DoPathfinding();
        }
    }

    private void FleeTargetUpdate()
    {
        // if the tank has reached its flee point
        if (GetDistanceFromObject(_fleeSettings.Target) <= _patrolSettings.PatrolPointThreshold)
        {
            // if enough time has passed, go back to patrolling
            if (_fleeSettings.HasFinishedFleeing())
            {
                GoToMode(ActionMode.Patrol);
            }
            else // regain health
            {
                _controller.RegenerateHealth();
            }
        }
        else // we need to move towards the flee target still
        {
            if (_currentMovementMode == MovementMode.Normal)
            {
                if (CanMove(Settings.MovementSettings.Forward))
                {
                    MotorComponent.RotateTowards(_fleeSettings.Target, Settings.MovementSettings.Rotation);
                    MotorComponent.Move(Settings.MovementSettings.Forward);
                }
                else
                {
                    _currentMovementMode = MovementMode.Pathfinding;
                }
            }
            else
            {
                DoPathfinding();
            }
        }
    }

    private void DoPathfinding()
    {
        MotorComponent.Rotate(-1 * Settings.MovementSettings.Rotation);

        if (CanMove(Settings.MovementSettings.Forward * Time.time))
        {
            if (_pathfindingExitTime == 0)
            {
                _pathfindingExitTime = _maxTimeDoingPathfinding;
            }

            MotorComponent.Move(Settings.MovementSettings.Forward);
            _pathfindingExitTime -= Time.deltaTime;

            if (_pathfindingExitTime <= 0)
            {
                _pathfindingExitTime = 0;
                _currentMovementMode = MovementMode.Normal;
            }
        }
    }

    private void GoToMode(ActionMode newMode)
    {
        switch (newMode)
        {
            case ActionMode.Flee:
                // Reset the manager and then set it up
                _fleeSettings.Reset();
                _fleeSettings.Setup(_currentTarget.position, MyTransform.position, _personality);
                break;

            case ActionMode.Patrol:
                break;

            default:
                break;
        }

        _currentActionMode = newMode;
    }

    private bool IsLookingAtPatrolPoint(Transform pointToCheck)
    {
        Quaternion directions = Quaternion.LookRotation(pointToCheck.position - MyTransform.position);

        return directions == MyTransform.rotation;
    }

    private bool CanMove(float speed)
    {
        RaycastHit ray;

        if (Physics.Raycast(MyTransform.position, MyTransform.forward, out ray, speed))
        {
            GameObject obj = ray.collider.gameObject;

            // return if the object hit was a player, flee target, or projectile (these are non blockers)
            return obj.IsOnSameLayer(ProjectSettings.Layers.Player) ||
                obj.IsOnSameLayer(ProjectSettings.Layers.Projectiles) ||
                (_fleeSettings.Target != null && obj == _fleeSettings.Target.gameObject);
        }
        // return true in all other cases
        return true;
    }

    private bool IsTargetWithinView(Transform target)
    {
        Vector3 targetPosition = target.position;
        Vector3 vectorToTarget = targetPosition - MyTransform.position;

        float angleToTarget = Vector3.Angle(vectorToTarget, MyTransform.forward);

        if (angleToTarget <= _visionSettings.LineOfSightAngle)
        {
            Ray ray = new Ray();

            ray.origin = MyTransform.position;
            ray.direction = vectorToTarget;

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, _visionSettings.HearingDistance))
            {
                if (hit.collider.gameObject.IsOnSameLayer(ProjectSettings.Layers.Player))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
