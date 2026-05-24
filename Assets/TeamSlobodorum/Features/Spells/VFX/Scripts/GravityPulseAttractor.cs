using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPulseAttractor : MonoBehaviour
{
    [Header("Pulse Area")]
    [SerializeField] private Transform center;
    [SerializeField] private float radius = 3.2f;
    [SerializeField] private float period = 1.1f;
    [SerializeField] private float startDelay = 0f;

    [Header("Pulse Pull")]
    [SerializeField] private float pulseDuration = 0.25f;

    [Tooltip("Velocity change applied every FixedUpdate during the pulse.")]
    [SerializeField] private float pullVelocityPerTick = 0.35f;

    [SerializeField] private float maxInwardSpeed = 8f;
    [SerializeField] private bool affectY = true;

    [Tooltip("X = normalized distance from center, Y = pull multiplier.")]
    [SerializeField]
    private AnimationCurve falloff =
        AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    [Header("Filtering")]
    [SerializeField] private LayerMask targetLayers = ~0;
    [SerializeField] private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;
    [SerializeField] private Transform ignoredRoot;

    [Header("Performance")]
    [SerializeField] private int maxColliders = 128;

    private Collider[] _hits;
    private readonly HashSet<Rigidbody> _targets = new();
    private Coroutine _loop;

    private void Awake()
    {
        _hits = new Collider[maxColliders];

        if (center == null)
            center = transform;
    }

    public void Initialize(Transform casterRoot)
    {
        ignoredRoot = casterRoot;
    }

    private void OnEnable()
    {
        _loop = StartCoroutine(PulseLoop());
    }

    private void OnDisable()
    {
        if (_loop != null)
        {
            StopCoroutine(_loop);
            _loop = null;
        }
    }

    private IEnumerator PulseLoop()
    {
        if (startDelay > 0f)
            yield return new WaitForSeconds(startDelay);

        while (true)
        {
            CollectTargets();

            yield return PullTargetsForDuration(pulseDuration);

            float wait = Mathf.Max(0f, period - pulseDuration);
            if (wait > 0f)
                yield return new WaitForSeconds(wait);
        }
    }

    private void CollectTargets()
    {
        Vector3 pullCenter = GetCenter();

        int count = Physics.OverlapSphereNonAlloc(
            pullCenter,
            radius,
            _hits,
            targetLayers,
            triggerInteraction
        );

        _targets.Clear();

        for (int i = 0; i < count; i++)
        {
            Collider hit = _hits[i];

            if (hit == null)
                continue;

            Rigidbody rb = hit.attachedRigidbody;

            if (rb == null || rb.isKinematic)
                continue;

            if (ignoredRoot != null && rb.transform.IsChildOf(ignoredRoot))
                continue;

            _targets.Add(rb);
        }
    }

    private IEnumerator PullTargetsForDuration(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector3 pullCenter = GetCenter();

            foreach (Rigidbody rb in _targets)
            {
                if (rb == null)
                    continue;

                ApplyPull(rb, pullCenter);
            }

            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    private void ApplyPull(Rigidbody rb, Vector3 pullCenter)
    {
        Vector3 toCenter = pullCenter - rb.worldCenterOfMass;

        if (!affectY)
            toCenter.y = 0f;

        float distance = toCenter.magnitude;

        if (distance <= 0.001f)
            return;

        float normalizedDistance = Mathf.Clamp01(distance / radius);
        float strength = falloff.Evaluate(normalizedDistance);

        Vector3 direction = toCenter / distance;
        Vector3 velocityChange = direction * pullVelocityPerTick * strength;

        rb.AddForce(velocityChange, ForceMode.VelocityChange);

        ClampInwardSpeed(rb, direction);
    }

    private void ClampInwardSpeed(Rigidbody rb, Vector3 inwardDirection)
    {

        Vector3 velocity = rb.linearVelocity;


        float inwardSpeed = Vector3.Dot(velocity, inwardDirection);

        if (inwardSpeed <= maxInwardSpeed)
            return;

        Vector3 excess = inwardDirection * (inwardSpeed - maxInwardSpeed);
        velocity -= excess;

        rb.linearVelocity = velocity;

    }

    private Vector3 GetCenter()
    {
        return center != null ? center.position : transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 c = center != null ? center.position : transform.position;

        Gizmos.color = new Color(0.3f, 0.7f, 1f, 0.35f);
        Gizmos.DrawWireSphere(c, radius);
    }
}