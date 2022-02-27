using UnityEngine;

namespace Cannon
{
    public class CannonTarget : MonoBehaviour
    {
        public bool IsRoot => _parent == null;
        
        private CannonTarget _parent;

        private void Awake()
        {
            _parent = null;
        }

        private void Start()
        {
            if (!IsRoot)
            {
                return;
            }
            AddComponentsToChildren();
            SetKinematicRigidBodyRecursive(true);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!IsRoot)
            {
                _parent.OnCollisionEnter(collision);
                return;
            }
            
            if (!collision.transform.TryGetComponent<CannonProjectile>(out _))
            {
                return;
            }
            SetKinematicRigidBodyRecursive(false);
        }

        private void SetKinematicRigidBodyRecursive(bool value)
        {
            var rigidBodyComponents = GetComponentsInChildren<Rigidbody>();
            foreach (var component in rigidBodyComponents)
            {
                component.isKinematic = value;
            }
        }

        // Black magic.
        private void AddComponentsToChildren()
        {
            var children = GetComponentsInChildren<Rigidbody>();
            foreach (var child in children)
            {
                var childGameObject = child.transform.gameObject;
                var cannonTarget = childGameObject.AddComponent<CannonTarget>();
                cannonTarget._parent = this;
            }
        }
    }
}
