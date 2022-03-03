using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Util;


namespace Cannon
{
    public class CannonShooting : MonoBehaviour
    {
        public Vector2 horizontalAngleRange;

        public float shotCooldown;

        public GameObject projectilePrefab;


        
        private Transform Target => 
            _currentTarget < _targets.Count ?
                _targets[_currentTarget] :
                null;
        private List<Transform> _targets;
        private int _currentTarget;

        private CannonRotation _cannonRotationController;
        private CannonState _state;

        private bool _isProjectileShot;
        private float _shotCooldownTimer;
        private Vector3 _projectileInitialVelocity;

        private Vector3 _forward;
        private const float CannonAngle = 45;


        private void Start()
        {
            _state = CannonState.Idle;
            _forward = transform.forward;
            _cannonRotationController = GetComponent<CannonRotation>();
            _targets = FindAllTargets();
            _shotCooldownTimer = 0;
            _currentTarget = -1; // Because +1 on first call.
        }
        
        private void Update()
        {
            switch (_state)
            {
                case CannonState.Rotating:
                    HandleStateRotating();
                    break;
                case CannonState.ReadyForAttack:
                    HandleStateReadyForAttack();
                    break;
                case CannonState.Idle:
                default:
                    HandleStateIdle();
                    break;
            }
        }

        private void HandleStateIdle()
        {
            var target = GetNextTarget();
            if (!target)
            {
                return;
            }
            
            SetupForShot();
            _state = CannonState.Rotating;
        }

        private void HandleStateReadyForAttack()
        {
            if (!_isProjectileShot)
            {
                ShootProjectile();
                _isProjectileShot = true;
                _shotCooldownTimer = 0;
            }

            if (_shotCooldownTimer < shotCooldown)
            {
                _shotCooldownTimer += Time.deltaTime;
                return;
            }
            
            _isProjectileShot = false;
            _state = CannonState.Idle;
        }
        
        private void HandleStateRotating()
        {
            if (_cannonRotationController.IsRotating)
            {
                return;
            }

            _state = CannonState.ReadyForAttack;
        }

        private void ShootProjectile()
        {
            var projectile = Instantiate(
                projectilePrefab, 
                _cannonRotationController.Position, 
                Quaternion.identity);
            var projectileBody = projectile.GetComponent<Rigidbody>();
            projectileBody.velocity = _projectileInitialVelocity;
        }

        private int CompareTargets(Transform left, Transform right)
        {
            // CompareTo is bad, I really miss `std::less`.
            var leftDistance  = Vector3Extensions.HorizontalDistance(
                left.position, _cannonRotationController.Position);
            var rightDistance = Vector3Extensions.HorizontalDistance(
                right.position, _cannonRotationController.Position);
            return leftDistance.CompareTo(rightDistance);
        }
        
        private List<Transform> FindAllTargets()
        {
            var targets = (
                from target in FindObjectsOfType<CannonTarget>() 
                where target.IsRoot
                select target.transform)
                .ToList();

            targets.Sort(CompareTargets);
            return targets;
        }

        private Transform GetNextTarget()
        {
            if (_currentTarget >= _targets.Count)
            {
                return null;
            }
            ++_currentTarget;
            return Target;
        }


        private void SetupForShot()
        {
            var cannonTarget = Target.GetComponent < CannonTarget>();
            var from = _cannonRotationController.Position;
            var to   = Target.transform.position + cannonTarget.hitboxOffset;
            var initialVelocity = GetInitialVelocity(from, to);
            
            var direction = to - from;
            direction.y = 0;
            
            var velocityDirection = new Vector3(
                direction.x,
                direction.magnitude * (float)Math.Tan(CoolMath.Radians(CannonAngle)),
                direction.z);
            _projectileInitialVelocity = initialVelocity * velocityDirection.normalized;

            var yaw = Vector3.Angle(direction, _forward);
            var sign = -Math.Sign(Vector3.Cross(direction, _forward).y);
            yaw *= sign;
            
            _cannonRotationController.SetRotation(yaw, CannonAngle);
        }

        private static float GetInitialVelocity(Vector3 from, Vector3 to)
        {
            // Derived from projectile motion formulas,
            // Simplified for angle = 45 degrees.
            // Took me a whole day to make at least something that can work.
            var s = Vector3Extensions.HorizontalDistance(from, to);
            var h = Vector3Extensions.VerticalDistance(from, to);
            var g = Math.Abs(Physics.gravity.y);
            var v0 = Math.Sqrt(g * s * s / (s - h));
            return (float) v0;
        }
    }
}
