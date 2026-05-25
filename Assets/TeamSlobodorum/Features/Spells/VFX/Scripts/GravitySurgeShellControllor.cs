using System.Collections;
using UnityEngine;

public class GravitySurgeShellController : MonoBehaviour
{
    [System.Serializable]
    public class ShellLayer
    {
        public Transform shell;

        [Header("Scale")]
        public float startScale = 6f;
        public float endScale = 0.5f;

        [Header("Timing")]
        public float delay = 0f;
        public float duration = 0.6f;
    }

    [Header("Shell Layers")]
    [SerializeField] private ShellLayer[] layers;

    [Header("Playback")]
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool loop = true;
    [SerializeField] private float loopDelay = 0.15f;

    private Coroutine _playRoutine;

    private void OnEnable()
    {
        if (playOnStart)
        {
            StartLoop();
        }
    }

    private void OnDisable()
    {
        StopLoop();
    }

    public void StartLoop()
    {
        StopLoop();
        _playRoutine = StartCoroutine(PlayLoopRoutine());
    }

    public void StopLoop()
    {
        if (_playRoutine != null)
        {
            StopCoroutine(_playRoutine);
            _playRoutine = null;
        }
    }

    public void PlayOnce()
    {
        StopLoop();
        _playRoutine = StartCoroutine(PlayOnceRoutine());
    }

    private IEnumerator PlayLoopRoutine()
    {
        do
        {
            yield return PlayOnceRoutine();

            if (loopDelay > 0f)
                yield return new WaitForSeconds(loopDelay);

        } while (loop);
    }

    private IEnumerator PlayOnceRoutine()
    {
        ResetLayersToStart();

        float longestTime = GetLongestLayerTime();

        foreach (ShellLayer layer in layers)
        {
            if (layer.shell != null)
            {
                StartCoroutine(AnimateLayer(layer));
            }
        }

        yield return new WaitForSeconds(longestTime);
    }

    private IEnumerator AnimateLayer(ShellLayer layer)
    {
        yield return new WaitForSeconds(layer.delay);

        float t = 0f;

        while (t < layer.duration)
        {
            float p = t / layer.duration;

            // Smoothstep easing
            float eased = p * p * (3f - 2f * p);

            float scale = Mathf.Lerp(layer.startScale, layer.endScale, eased);
            layer.shell.localScale = Vector3.one * scale;

            t += Time.deltaTime;
            yield return null;
        }

        layer.shell.localScale = Vector3.one * layer.endScale;
    }

    private void ResetLayersToStart()
    {
        foreach (ShellLayer layer in layers)
        {
            if (layer.shell != null)
            {
                layer.shell.localScale = Vector3.one * layer.startScale;
            }
        }
    }

    private float GetLongestLayerTime()
    {
        float longest = 0f;

        foreach (ShellLayer layer in layers)
        {
            float totalTime = layer.delay + layer.duration;

            if (totalTime > longest)
                longest = totalTime;
        }

        return longest;
    }
}