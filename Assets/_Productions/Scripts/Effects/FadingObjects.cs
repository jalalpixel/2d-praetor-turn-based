using UnityEngine;
using System.Collections;

public class FadingObjects : MonoBehaviour
{
    public float durToFadeAway; // Total duration to complete the fade effect in seconds
    public bool isDelayToFadeAway = false; // Flag to delay the fading process at start
    public bool isObjectParticleSystem; // Option if object is a particle system
    public float delayDurationToFadeAway; // Duration to delay the fading process
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private ParticleSystem particleSystem;

    private Color originalColor;
    private float initTransparency;
    private ParticleSystem.EmissionModule emissionModule;
    private float initEmissionRate;

    private void OnEnable()
    {
        if (isObjectParticleSystem)
        {
            if (particleSystem == null)
                particleSystem = GetComponent<ParticleSystem>();

            // Capture the original emission rate
            emissionModule = particleSystem.emission;
            initEmissionRate = emissionModule.rateOverTime.constant;
        }
        else
        {
            // Capture the original color and transparency
            if (originalColor == default) // Check if originalColor is uninitialized
            {
                originalColor = spriteRenderer.color;
                initTransparency = originalColor.a;
            }

            // Reset the sprite's color to the original color with the initial transparency
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, initTransparency);
        }

        // Start the fading process
        StartCoroutine(StartFade());
    }

    private void OnDisable()
    {
        if (isObjectParticleSystem)
        {
            // Reset the emission rate to the initial value
            SetEmissionRate(initEmissionRate);
        }
        else
        {
            // Reset the sprite's color to the original color with the initial alpha value
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, initTransparency);
        }
    }

    private IEnumerator StartFade()
    {
        if (isDelayToFadeAway && delayDurationToFadeAway > 0f)
        {
            yield return new WaitForSeconds(delayDurationToFadeAway);
        }

        float remainingDuration = durToFadeAway - delayDurationToFadeAway;
        if (remainingDuration > 0f)
        {
            if (isObjectParticleSystem)
            {
                StartCoroutine(FadeOutParticleSystem(remainingDuration));
            }
            else
            {
                StartCoroutine(FadeOutSprite(remainingDuration));
            }
        }
        else
        {
            if (isObjectParticleSystem)
            {
                SetEmissionRate(0f);
            }
            else
            {
                // If the remaining duration is zero or negative, immediately set alpha to 0
                Color finalColor = originalColor;
                finalColor.a = 0f;
                spriteRenderer.color = finalColor;
            }
        }
    }

    private IEnumerator FadeOutSprite(float remainingDuration)
    {
        float timer = 0f;

        while (timer < remainingDuration)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / remainingDuration;

            // Interpolate the alpha directly from the original alpha to 0
            Color fadeColor = originalColor;
            fadeColor.a = Mathf.Lerp(initTransparency, 0f, normalizedTime);
            spriteRenderer.color = fadeColor;

            yield return null;
        }

        // Ensure the final alpha value is exactly 0
        Color finalColor = originalColor;
        finalColor.a = 0f;
        spriteRenderer.color = finalColor;
    }

    private IEnumerator FadeOutParticleSystem(float remainingDuration)
    {
        float timer = 0f;

        while (timer < remainingDuration)
        {
            timer += Time.deltaTime * 2;
            float normalizedTime = timer / remainingDuration;

            // Interpolate the emission rate from the initial rate to 0
            float emissionRate = Mathf.Lerp(initEmissionRate, 0f, normalizedTime);
            SetEmissionRate(emissionRate);

            yield return null;
        }

        // Ensure the final emission rate is exactly 0
        SetEmissionRate(0f);
    }

    private void SetEmissionRate(float emissionRate)
    {
        var rate = emissionModule.rateOverTime;
        rate.constant = emissionRate;
        emissionModule.rateOverTime = rate;
    }
}
