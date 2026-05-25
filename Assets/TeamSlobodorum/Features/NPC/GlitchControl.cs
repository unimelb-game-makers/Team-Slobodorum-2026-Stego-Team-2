using System.Collections;
using UnityEngine;

public class GlitchControl : MonoBehaviour
{
    [Header("Trigger Settings")]
    public float glitchChance = 0.1f;
    public float loopWaitTime = 0.1f;

    [Header("Intensity Ranges")]
    public Vector2 glitchIntensityRange = new Vector2(0.07f, 0.1f);
    public Vector2 glowDropRange = new Vector2(0.14f, 0.44f); // Lowers glow during glitch
    
    [Header("Brightness Surge")]
    [Tooltip("How much the brightness multiplies during a glitch")]
    public Vector2 brightnessSurgeRange = new Vector2(2.0f, 5.0f); 

    [Header("Timing")]
    public Vector2 glitchDurationRange = new Vector2(0.05f, 0.1f);

    public float originalGlowIntensity;
    public float originalBrightness ;
    Material hologramMaterial;
    WaitForSeconds glitchLoopWait;

    void OnEnable()
    {
        hologramMaterial = GetComponent<Renderer>().material;
        glitchLoopWait = new WaitForSeconds(loopWaitTime);
        StartCoroutine(GlitchStart());

    }

    IEnumerator GlitchStart()
    {
        while (true)
        {
            float glitchTest = Random.Range(0f, 1f);

            if (glitchTest <= glitchChance)
            {
                // 2. Apply random glitch values based on Inspector variables
                hologramMaterial.SetFloat("_GlitchIntensity", Random.Range(glitchIntensityRange.x, glitchIntensityRange.y));
                
                // Drop the glow
                hologramMaterial.SetFloat("_GlowIntensity", originalGlowIntensity * Random.Range(glowDropRange.x, glowDropRange.y));
                
                // Spike the brightness (Power Surge)
                hologramMaterial.SetFloat("_Brightness", originalBrightness * Random.Range(brightnessSurgeRange.x, brightnessSurgeRange.y));

                // 3. Wait for the random glitch duration
                yield return new WaitForSeconds(Random.Range(glitchDurationRange.x, glitchDurationRange.y));

                // 4. Restore the original clean state
                hologramMaterial.SetFloat("_GlitchIntensity", 0f);
                hologramMaterial.SetFloat("_GlowIntensity", originalGlowIntensity);
                hologramMaterial.SetFloat("_Brightness", originalBrightness);
            }

            yield return glitchLoopWait;
        }
    }
    void Onsable()
    {
        StopCoroutine(GlitchStart());
    }
}