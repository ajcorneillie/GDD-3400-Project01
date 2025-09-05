using System.Collections.Generic;
using UnityEngine;

namespace GDD3400.Project01
{
    [RequireComponent(typeof(Rigidbody))]
    public class SimpleSheep : MonoBehaviour
    {
        public enum SheepAction
        {
            Wandering,
            Fleeing,
            Flocking,
            Resting
        }

        [Header("Perception")]
        [SerializeField] private float _sightRadius = 10f;
        [SerializeField] private float _friendRadius = 5f;

        [Header("Locomotion")]
        [SerializeField] private float _maxSpeed = 3f;
        [SerializeField] private float _turnRate = 6f;
        [SerializeField] private float _wanderTurnRate = 2f;
        [SerializeField] private float _stoppingDistance = 2f;

        private string _friendTag = "Friend";
        private string _threatTag = "Threat";
        private string _safeZoneTag = "SafeZone";

        private SheepAction _currentAction = SheepAction.Wandering;

        private Rigidbody _rb;
        private Vector3 _velocity;
        private Vector3 _target;

        private float _actionTimer = 0f;

        // buffers
        readonly Collider[] _tmpTargets = new Collider[32];
        private List<Collider> _filteredTargets = new List<Collider>();


        #region Initialization
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }
        #endregion

        #region  Decision Making
        private void Update()
        {
            //TODO: Optionally use the action timer to limit the number of actions per second
            // Since movement is handled in FixedUpdate, we can limit the number of actions per second here

            // Clear the filtered targets
            _filteredTargets.Clear();
            
            // Primary Behavior: Check if the sheep is in a safe zone
            if (InSafeZone())
            {
                DoRest();
                return;
            }
            // Secondary Behavior: Check if the sheep has detected a threat
            else if (ThreatDetected())
            {
                DoFlee();
                return;
            }
            // Tertiary Behavior: Check if the sheep has detected a friend
            else if (FriendDetected())
            {
                DoFlock();
                return;
            }
            // Default Behavior: Wander
            else
            {
                DoWander();
            }
        }
        #endregion

        #region Perception
        /// <summary>
        /// Checks if the sheep is in a safe zone, uses a non-alloc buffer to avoid creating garbage
        /// </summary>
        /// <returns></returns>
        private bool InSafeZone()
        {
            // Collect all colliders around the sheep
            int n = Physics.OverlapSphereNonAlloc(transform.position, 0.5f, _tmpTargets);
            for (int i = 0; i < n; i++)
            {
                var c = _tmpTargets[i];

                // If the collider is not null and has the safe zone tag, add it to the filtered targets
                if (c != null && c.CompareTag(_safeZoneTag)) 
                {
                    _filteredTargets.Add(c);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if the sheep has detected a threat, uses a non-alloc buffer to avoid creating garbage
        /// </summary>
        /// <returns></returns>
        private bool ThreatDetected()
        {
            // Collect all colliders within the sight radius
            int n = Physics.OverlapSphereNonAlloc(transform.position, _sightRadius, _tmpTargets);
            for (int i = 0; i < n; i++)
            {
                var c = _tmpTargets[i];

                // If the collider is not null and has the threat tag, add it to the filtered targets
                if (c != null && c.CompareTag(_threatTag)) 
                { 
                    _filteredTargets.Add(c);
                }
            }

            // Return true if there are any filtered targets
            return _filteredTargets.Count > 0;
        }

        /// <summary>
        /// Checks if the sheep has detected a friend, uses a non-alloc buffer to avoid creating garbage
        /// </summary>
        /// <returns></returns>
        private bool FriendDetected()
        {
            // Collect all colliders within the sight radius
            int n = Physics.OverlapSphereNonAlloc(transform.position, _sightRadius, _tmpTargets);
            for (int i = 0; i < n; i++)
            {
                var c = _tmpTargets[i];

                // If the collider is not null and has the friend tag, add it to the filtered targets
                if (c != null && c.CompareTag(_friendTag)) 
                { 
                    _filteredTargets.Add(c);
                }
            }

            // Return true if there are any filtered targets
            return _filteredTargets.Count > 0;
        }

        #endregion

        #region Actions
        private void DoRest()
        {
            // Update the current action
            _currentAction = SheepAction.Resting;

            if (_filteredTargets.Count == 0) return;

            _target = _filteredTargets[0].transform.position;
        }

        private void DoFlee()
        {
            // Update the current action
            _currentAction = SheepAction.Fleeing;

            if (_filteredTargets.Count == 0) return;

            // TODO: Flee Logic - Using the filtered targets, pick the appopriate threat and move away from it
        
        }

        private void DoFlock()
        {
            // Update the current action
            _currentAction = SheepAction.Flocking;

            if (_filteredTargets.Count == 0) return;

            // TODO: Flock Logic - Using the filtered targets, pick the appopriate friend and move toward it

        }

        private void DoWander()
        {
            // Update the current action
            _currentAction = SheepAction.Wandering;

            // TODO: Wander Logic - Pick a new random wander target using any method you wish

        }

        #endregion

        #region Movement
        private void FixedUpdate()
        {
            // First check if the target position is not zero and the distance is greater than the stopping distance
            if (_target != Vector3.zero && Vector3.Distance(transform.position, _target) > _stoppingDistance)
            {
                // Calculate the direction to the target position
                Vector3 direction = (_target - transform.position).normalized;

                // Calculate the movement vector
                _velocity = direction * Mathf.Min(_maxSpeed, Vector3.Distance(transform.position, _target));

                // Slowly lerp the transform rotation to face the direction
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _turnRate);
            }
            else
            {
                _velocity = Vector3.zero;
            }

            _rb.linearVelocity = _velocity;
        }
        #endregion

        #region Gizmos

        private void OnDrawGizmosSelected()
        {
            // Colors
            Color blue = new Color(0.2f, 0.6f, 1f, 1f);
            Color green = new Color(0.2f, 1f, 0.4f, 1f);
            Color orange = new Color(1f, 0.55f, 0f, 1f);
            Color red = new Color(1f, 0f, 0f, 1f);

            // Ground circles (XZ)
            DrawCircleXZ(transform.position, _sightRadius, blue);
            DrawCircleXZ(transform.position, _friendRadius, green);

            switch (_currentAction)
            {
                case SheepAction.Wandering:
                    Gizmos.color = orange;
                    break;
                case SheepAction.Fleeing:
                    Gizmos.color = red;
                    break;
                case SheepAction.Flocking:
                    Gizmos.color = green;
                    break;
            }

            if (_filteredTargets != null)
            {
                foreach (Collider filteredTarget in _filteredTargets)
                {
                    Gizmos.DrawLine(transform.position, filteredTarget.transform.position);
                }
            }

            DrawCircleXZ(transform.position, 1f, Gizmos.color);

            if (_velocity != Vector3.zero)
            {
                Gizmos.DrawLine(transform.position, transform.position + _velocity.normalized * 2.5f);
            }
        }

        private void DrawCircleXZ(Vector3 center, float radius, Color color, int segments = 48)
        {
            Gizmos.color = color;
            Vector3 prev = center + new Vector3(radius, 0f, 0f);
            float step = Mathf.PI * 2f / segments;

            for (int i = 1; i <= segments; i++)
            {
                float a = step * i;
                Vector3 next = center + new Vector3(Mathf.Cos(a) * radius, 0f, Mathf.Sin(a) * radius);
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
        }

        #endregion
    }
}
