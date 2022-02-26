using UnityEngine;

// Minecraft-style camera movement.
// Stolen from here and rewritten with my own (cursed) style.
// https://gist.github.com/ashleydavis/f025c03a9221bc840a2b
// Controls:
// WASD/Arrows - Movement.
// Shift/Space - Fly Up/Down respectively.
// Hold Right Mouse - Enables camera rotation by mouse.
namespace Util
{
    public class FreeCameraMovement : MonoBehaviour
    {
        public float defaultMovementSpeed;
        public float fastMovementSpeed;
    
        public float freeLookSensitivity;
        
        private float MovementSpeed => _isFastMode ? fastMovementSpeed : defaultMovementSpeed;
        
        // When looking up. Should be -89.9 but Unity.
        private const float UpperRotationBorder = 270.1f;
        
        // When looking down.
        private const float LowerRotationBorder = 89.9f;

        private bool _isFreeLookingEnabled;
        private bool _isFastMode;
    
        private void Start()
        {
            _isFreeLookingEnabled = false;
            _isFastMode = false;
        }

        private void OnDisable()
        {
            DisableFreeLooking();
        }
    
    
        private void Update()
        {
            CheckForFastMode();
        
            HandleVerticalMovement();
            HandleHorizontalMovement();
        
            HandleCameraRotation();
        }

        private void CheckForFastMode()
        {
            _isFastMode = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        }
    
        private void HandleHorizontalMovement()
        {
            var cameraTransform = transform;
            var displacement = Time.deltaTime * MovementSpeed;

            var horizontalForwardVector = cameraTransform.forward;
            horizontalForwardVector.y = 0;

            var direction = Vector3.zero;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                direction += cameraTransform.right;
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                direction -= cameraTransform.right;
            }
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                direction += horizontalForwardVector.normalized;
            }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                direction -= horizontalForwardVector.normalized;
            }
        
            cameraTransform.position += displacement * direction;
        }
    
        private void HandleVerticalMovement()
        {
            var displacement = Time.deltaTime * MovementSpeed;
            var direction = Vector3.zero;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                direction = Vector3.down;
            }
            if (Input.GetKey(KeyCode.Space))
            {
                direction = Vector3.up;
            }
        
            var cameraTransform = transform;
            cameraTransform.position += displacement * direction;
        }
    
    
        private void HandleCameraRotation()
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                EnableFreeLooking();
            }
            else if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                DisableFreeLooking();
            }
        
            if (_isFreeLookingEnabled)
            {
                RotateCamera();
            }
        }

    
        private void EnableFreeLooking()
        {
            _isFreeLookingEnabled = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    
    
        private void DisableFreeLooking()
        {
            _isFreeLookingEnabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

    
        private void RotateCamera()
        {
            var oldAngles = transform.localEulerAngles;

            var newRotationX = oldAngles.y + Input.GetAxis("Mouse X") * freeLookSensitivity;
            var newRotationY = oldAngles.x - Input.GetAxis("Mouse Y") * freeLookSensitivity;

            var isUpperHemisphere = oldAngles.x >= 270 && oldAngles.x <= 360;
            var isLowerHemisphere = oldAngles.x >= 0 && oldAngles.x <= 90;
            
            // To avoid flickering.
            if (isUpperHemisphere && newRotationY < UpperRotationBorder)
            {
                newRotationY = UpperRotationBorder;
            }
            if (isLowerHemisphere && newRotationY > LowerRotationBorder)
            {
                newRotationY = LowerRotationBorder;
            }
            
            transform.localEulerAngles = new Vector3(newRotationY, newRotationX, 0f);
        }
    }
}
