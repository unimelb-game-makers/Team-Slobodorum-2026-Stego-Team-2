using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPulseAttractor : MonoBehaviour
{
    [Header("Pulse Area")]
    [SerializeField] private Transform center;
    [SerializeField] private float radius = 7f;
    [SerializeField] private float pulseInterval = 0.5f;
    [SerializeField] private float startDelay = 0f;

    [Header("Pulse Pull")]
    [Tooltip("Velocity change applied once per pulse.")]
    [SerializeField] private float pullVelocityPerPulse = 1.5f;

    [SerializeField] private float maxInwardSpeed = 50f;
    [SerializeField] private bool affectY = false;

    [Tooltip("X = normalized distance from center, Y = pull multiplier.")]
    [SerializeField]
    private AnimationCurve falloff =
        AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    [Header("Filtering")]
    [SerializeField] private LayerMask targetLayers = ~0;
    [SerializeField] private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide;
    [SerializeField] private Transform ignoredRoot;

    [Header("Performance")]
    [SerializeField] private int maxColliders = 128;

    private Collider[] _hits;
    private readonly HashSet<Rigidbody> _targets = new();
    private Coroutine _loop;

    private void Awake()
    {
        if (center == null)
            center = transform;

        _hits = new Collider[Mathf.Max(1, maxColliders)];
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
            ApplyPulseToTargets();

            if (pulseInterval > 0f)
                yield return new WaitForSeconds(pulseInterval);
            else
                yield return null;
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
        Debug.Log("Targets collected: " +  _targets.Count);
    }

    private void ApplyPulseToTargets()
    {
        Vector3 pullCenter = GetCenter();

        foreach (Rigidbody rb in _targets)
        {
            if (rb == null)
                continue;

            ApplyPull(rb, pullCenter);
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
        Vector3 velocityChange = direction * pullVelocityPerPulse * strength;

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