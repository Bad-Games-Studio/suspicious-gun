using System;
using UnityEngine;

namespace Cannon
{
    // i hated making that part cos every single f- guide just spreads wrong info.
    // docs are pretty good tho, /shrug
    public class CannonRotation : MonoBehaviour
    {
        public GameObject carriage;
        public GameObject barrel;
        
        public float animationLengthSeconds;

        public bool IsRotating => _animationTime < animationLengthSeconds;

        public float Yaw => carriage.transform.rotation.eulerAngles.y;
        public float Pitch => -barrel.transform.rotation.eulerAngles.x;

        public Vector3 Position => barrel.transform.position;

        private float _animationTime;
        
        private Quaternion _sourceCarriageRotation;
        private Quaternion _targetCarriageRotation;
        
        private Quaternion _sourceBarrelRotation;
        private Quaternion _targetBarrelRotation;

        private CannonShooting _cannonParameters;

        private void Start()
        {
            _animationTime = animationLengthSeconds + 1;
            _sourceCarriageRotation = carriage.transform.rotation;
            _targetCarriageRotation = _sourceCarriageRotation;
            
            _sourceBarrelRotation = barrel.transform.rotation;
            _targetBarrelRotation = _sourceBarrelRotation;

            _cannonParameters = GetComponent<CannonShooting>();
        }

        private void Update()
        {
            if (_animationTime > animationLengthSeconds)
            {
                return;
            }

            var t = _animationTime / animationLengthSeconds;
            carriage.transform.rotation = Quaternion.Slerp(_sourceCarriageRotation, _targetCarriageRotation, t);
            barrel.transform.rotation = Quaternion.Slerp(_sourceBarrelRotation, _targetBarrelRotation, t);

            _animationTime += Time.deltaTime;
        }
        
        public void SetRotation(float yaw, float pitch)
        {
            yaw = ValidateYaw(yaw);
            pitch = ValidatePitch(pitch);
            
            _animationTime = 0;
            
            _sourceCarriageRotation = carriage.transform.rotation;
            _targetCarriageRotation = Quaternion.Euler(0, yaw, 0);

            // Negating pitch because I want it to rotate up only.
            _sourceBarrelRotation = barrel.transform.rotation;
            _targetBarrelRotation = Quaternion.Euler(-pitch, yaw, 0);
        }

        private float ValidateYaw(float yaw)
        {
            if (yaw < _cannonParameters.horizontalAngleRange.x)
            {
                return _cannonParameters.horizontalAngleRange.x;
            }

            if (yaw > _cannonParameters.horizontalAngleRange.y)
            {
                return _cannonParameters.horizontalAngleRange.y;
            }

            return yaw;
        }

        private float ValidatePitch(float pitch)
        {
            if (pitch < _cannonParameters.verticalAngleRange.x)
            {
                return _cannonParameters.verticalAngleRange.x;
            }

            if (pitch > _cannonParameters.verticalAngleRange.y)
            {
                return _cannonParameters.verticalAngleRange.y;
            }

            return pitch;
        }
    }
}
