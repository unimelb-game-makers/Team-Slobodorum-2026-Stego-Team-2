using UnityEngine;
using UnityEngine.InputSystem;

namespace TeamSlobodorum.Entities.Player
{
    public class PlayerAimController : MonoBehaviour
    {
        public enum CouplingMode
        {
            Coupled,
            CoupledWhenMoving,
            Decoupled
        }

        [Tooltip("How the player's rotation is coupled to the camera's rotation.  Three modes are available:\n"
                 + "<b>Coupled</b>: The player rotates with the camera.  Sideways movement will result in strafing.\n"
                 + "<b>Coupled When Moving</b>: Camera can rotate freely around the player when the player is stationary, "
                 + "but the player will rotate to face camera forward when it starts moving.\n"
                 + "<b>Decoupled</b>: The player's rotation is independent of the camera's rotation.")]
        public CouplingMode playerRotation;

        [Tooltip("How fast the player rotates to face the camera direction when the player starts moving.  "
                 + "Only used when Player Rotation is Coupled When Moving.")]
        public float rotationDamping = 0.2f;
        public float aimSensitivity = 10f;

        private PlayerMovement _playerMovement;
        private Quaternion _desiredWorldRotation;
        private Camera _mainCamera;
        private Vector2 _aimRotation;
        
        private void Start()
        {
            _playerMovement = GetComponentInParent<PlayerMovement>();
            _mainCamera = Camera.main;
        }

        private void FixedUpdate()
        {
            switch (playerRotation)
            {
                case CouplingMode.Coupled:
                {
                    _playerMovement.IsStrafeMode = true;
                    UpdatePlayerRotation();
                    break;
                }
                case CouplingMode.CoupledWhenMoving:
                {
                    // If the player is moving, rotate its yaw to match the camera direction,
                    // otherwise let the camera orbit
                    _playerMovement.IsStrafeMode = true;
                    if (_playerMovement.IsMoving)
                    {
                        UpdatePlayerRotation();
                    }
                    break;
                }
                case CouplingMode.Decoupled:
                {
                    _playerMovement.IsStrafeMode = false;
                    break;
                }
            }
        }

        /// <summary>Recenters the player to match the camera rotation</summary>
        public void RecenterPlayer()
        {
            var forward = _mainCamera.transform.forward;
            forward.y = 0;
            _playerMovement.Rigidbody.MoveRotation(Quaternion.LookRotation(forward));
            _aimRotation = new Vector2(0, _playerMovement.Rigidbody.rotation.eulerAngles.y);
            transform.localRotation = Quaternion.identity;
        }

        private void UpdatePlayerRotation()
        {
            _playerMovement.Rigidbody.MoveRotation(Quaternion.Euler(0f, _aimRotation.y, 0f));
        }
        
        public void OnLook(InputValue value)
        {
            if (playerRotation == CouplingMode.Coupled)
            {
                var lookInput = value.Get<Vector2>();
                if (lookInput.sqrMagnitude > 0.01f)
                {
                    _aimRotation.y += lookInput.x * aimSensitivity * Time.deltaTime;
                    _aimRotation.x -= lookInput.y * aimSensitivity * Time.deltaTime;

                    // Clamp the vertical rotation to prevent flipping
                    _aimRotation.x = Mathf.Clamp(_aimRotation.x, -90f, 90f);

                    transform.localRotation = Quaternion.Euler(_aimRotation.x, 0f, 0f);
                }
            }
        }
    }
}