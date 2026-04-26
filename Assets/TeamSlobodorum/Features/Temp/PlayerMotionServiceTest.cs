using TeamSlobodorum.Spells.Motion;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TeamSlobodorum.Entities.Player
{
    public class PlayerMotionServiceTest : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerMotionService movementService;

        [Header("Trigger")]
        [SerializeField] private bool submitOnStart = false;
        [SerializeField] private Key triggerKey = Key.T;

        [Header("Motion")]
        [SerializeField] private Vector3 direction = Vector3.forward;
        [SerializeField] private bool useLocalDirection = true;
        [SerializeField] private bool normalizeDirection = true;
        [SerializeField] private float acceleration = 8f;
        [SerializeField] private float duration = 1f;

        [Header("Override")]
        [SerializeField] private bool overrideXZ = false;
        [SerializeField] private bool overrideY = false;

        private void Awake()
        {
            if (movementService == null)
                movementService = GetComponent<PlayerMotionService>();
        }

        private void Start()
        {
            if (submitOnStart)
                SubmitTestMotion();
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current[triggerKey].wasPressedThisFrame)
                SubmitTestMotion();
        }

        [ContextMenu("Submit Test Motion")]
        public void SubmitTestMotion()
        {
            if (movementService == null)
            {
                Debug.LogWarning("PlayerMotionServiceTester: No PlayerMotionService found.", this);
                return;
            }

            Vector3 worldDirection = useLocalDirection
                ? transform.TransformDirection(direction)
                : direction;

            if (normalizeDirection && worldDirection.sqrMagnitude > 0.0001f)
                worldDirection.Normalize();

            Vector3 worldAcceleration = worldDirection * acceleration;

            movementService.Submit(worldAcceleration, duration, overrideXZ, overrideY);
        }
    }
}