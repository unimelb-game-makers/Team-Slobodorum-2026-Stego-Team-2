using System.Collections;
using UnityEngine;

public class LoopingDistortionSphereShrink : MonoBehaviour
{
    [Header("Distortion Target")]
    [SerializeField] private Transform distortionSphere;
    [SerializeField] private Renderer distortionRenderer;

    [Header("Core Target")]
    [SerializeField] private Transform core;
    [SerializeField] private float coreStartScale = 0f;
    [SerializeField] private float coreTargetScale = 1f;
    [SerializeField] private float coreExpandDuration = 0.35f;
    [SerializeField]
    private AnimationCurve coreExpandCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Shader Property")]
    [Tooltip("Must match the Reference name of your Shader Graph alpha property.")]
    [SerializeField] private string alphaPropertyName = "_DistortionStrength";

    [Header("Distortion Scale")]
    [Tooltip("Full sphere diameter scale at the start of each pulse.")]
    [SerializeField] private float startScale = 6.4f;

    [Tooltip("Small sphere scale at the end of each pulse.")]
    [SerializeField] private float endScale = 0.2f;

    [Header("Timing")]
    [SerializeField] private float interval = 1f;

    [Header("Starting Delay")]
    [SerializeField] private float startTime = 1f;

    [Tooltip("How much of the interval is spent shrinking.")]
    [SerializeField] private float shrinkDuration = 1f;

    [Header("Alpha Fade")]
    [SerializeField] private float maxDistortion = 0.3f;
    [SerializeField] private float fadeInDuration = 0.15f;
    [SerializeField]
    private AnimationCurve fadeInCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Shrink Curve")]
    [SerializeField]
    private AnimationCurve shrinkCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Coroutine _routine;
    private MaterialPropertyBlock _block;
    private int _alphaId;

    private void Awake()
    {
        _block = new MaterialPropertyBlock();
        _alphaId = Shader.PropertyToID(alphaPropertyName);
    }

    private void OnEnable()
    {
        if (distortionRenderer == null && distortionSphere != null)
            distortionRenderer = distortionSphere.GetComponent<Renderer>();

        if (distortionSphere != null)
        {
            distortionSphere.localScale = Vector3.one * startScale;
            ShowDistortionSphere(false);
            SetDistortion(0f);
        }

        if (core != null)
        {
            core.localScale = Vector3.one * coreStartScale;
        }

        _routine = StartCoroutine(LoopRoutine());
    }

    private void OnDisable()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }
    }

    private IEnumerator LoopRoutine()
    {
        Coroutine coreRoutine = null;

        if (core != null)
            coreRoutine = StartCoroutine(ExpandCore());

        if (startTime > 0f)
            yield return new WaitForSeconds(startTime);

        if (coreRoutine != null)
            yield return coreRoutine;

        while (true)
        {
            distortionSphere.localScale = Vector3.one * startScale;
            SetDistortion(0f);
            ShowDistortionSphere(true);

            yield return ShrinkOnce();

            SetDistortion(0f);
            ShowDistortionSphere(false);
            distortionSphere.localScale = Vector3.one * startScale;

            float wait = Mathf.Max(0f, interval - shrinkDuration);

            if (wait > 0f)
                yield return new WaitForSeconds(wait);
        }
    }

    private IEnumerator ExpandCore()
    {
        float t = 0f;

        while (t < coreExpandDuration)
        {
            float p = coreExpandDuration <= 0f ? 1f : t / coreExpandDuration;
            float eased = coreExpandCurve.Evaluate(p);

            float scale = Mathf.Lerp(coreStartScale, coreTargetScale, eased);
            core.localScale = Vector3.one * scale;

            t += Time.deltaTime;
            yield return null;
        }

        core.localScale = Vector3.one * coreTargetScale;
    }

    private IEnumerator ShrinkOnce()
    {
        float t = 0f;

        while (t < shrinkDuration)
        {
            float p = shrinkDuration <= 0f ? 1f : t / shrinkDuration;
            float shrinkEased = shrinkCurve.Evaluate(p);

            float scale = Mathf.Lerp(startScale, endScale, shrinkEased);
            distortionSphere.localScale = Vector3.one * scale;

            float fadeP = fadeInDuration <= 0f ? 1f : Mathf.Clamp01(t / fadeInDuration);
            float fadeEased = fadeInCurve.Evaluate(fadeP);
            SetDistortion(maxDistortion * fadeEased);

            t += Time.deltaTime;
            yield return null;
        }

        distortionSphere.localScale = Vector3.one * endScale;
    }

    private void ShowDistortionSphere(bool show)
    {
        if (distortionSphere != null)
            distortionSphere.gameObject.SetActive(show);
    }

    private void SetDistortion(float distortion)
    {
        if (distortionRenderer == null)
            return;

        distortionRenderer.GetPropertyBlock(_block);
        _block.SetFloat(_alphaId, distortion);
        distortionRenderer.SetPropertyBlock(_block);
    }
}