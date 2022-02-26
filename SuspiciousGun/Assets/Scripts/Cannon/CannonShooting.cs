using System.Collections.Generic;
using System.Linq;
using Level;
using UnityEngine;

namespace Cannon
{
    public class CannonShooting : MonoBehaviour
    {
        public Vector2 horizontalAngleRange;
        public Vector2 verticalAngleRange;

        public Vector2 initialVelocityRange;

        public bool CanAttack => _targets.Count > 0;
        
        private List<Transform> _targets;

        private CannonRotation _cannonRotation;
        
        private void Start()
        {
            _cannonRotation = GetComponent<CannonRotation>();
            _targets = FindAllTargets();
        }
        
        private void Update()
        {
            if (_cannonRotation.IsRotating)
            {
                return;
            }

            if (CanAttack)
            {
                SetupForShot();
            }
        }

        private static List<Transform> FindAllTargets()
        {
            var targets = new List<Transform>();
            foreach (var targetList in FindObjectsOfType<RandomTargetsGeneration>())
            {
                targets.AddRange(targetList.transform.Cast<Transform>());
            }
            return targets;
        }

        private Transform GetNextTarget()
        {
            if (_targets.Count == 0)
            {
                return null;
            }
            var target = _targets[_targets.Count - 1];
            _targets.RemoveAt(_targets.Count - 1);
            return target;
        }

        private void SetupForShot()
        {
            var target = GetNextTarget();
        }
    }
}
