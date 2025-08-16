using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class Utils
{
    public static void PlayAudioClip(AudioClip clip, float volume = 1f, float pitchVariance = 0.1f)
    { 
        if (clip == null) return;

        GameObject tempGO = new GameObject("TempAudio");
        if (Camera.main != null)
            tempGO.transform.position = Camera.main.transform.position;

        tempGO.transform.parent = null;
        PersistentVariables pv = GameObject.FindGameObjectWithTag("PVars").GetComponent<PersistentVariables>();

        // Set up AudioSource
        AudioSource audioSource = tempGO.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume * pv.sfxVolume;

        // random pitch
        audioSource.pitch = UnityEngine.Random.Range(1 - pitchVariance, 1 + pitchVariance);

        audioSource.spatialBlend = 0f; // 2D sound
        audioSource.Play();

        // Destroy after the adjusted length
        UnityEngine.Object.Destroy(tempGO, clip.length / audioSource.pitch);
    }

    public static void SpawnBurstParticle(GameObject particlePrefab, Vector3 position, Color color = default)
    {
        if (particlePrefab == null) return;
        if (color == default) color = Color.white;

        // Instantiate the particle prefab
        GameObject particleInstance = UnityEngine.Object.Instantiate(particlePrefab, position, Quaternion.identity);

        // Get the ParticleSystem component
        ParticleSystem ps = particleInstance.GetComponent<ParticleSystem>();
        if (ps == null)
        {
            Debug.LogWarning("Prefab has no ParticleSystem component!");
            UnityEngine.Object.Destroy(particleInstance);
            return;
        }

        // set color
        var main = ps.main;
        main.startColor = color;

        // Play it (in case it's not already set to play on awake)
        ps.Play();

        // Schedule destruction when it's done
        UnityEngine.Object.Destroy(particleInstance, ps.main.duration + ps.main.startLifetime.constantMax);
    }

    public static void SpawnFloatingText(GameObject textPrefab, Vector3 position, string textValue, float upwardForce = 3f, float sidewaysMax = 0.3f, float torqueForce = 5f, float lifetime = 0.8f, float fadeDuration = 0.4f, Color color = default)
    {
        if (textPrefab == null) return;

        // Spawn prefab
        GameObject textInstance = UnityEngine.Object.Instantiate(textPrefab, position, Quaternion.identity);

        // Get text component
        TextMeshPro tmp = textInstance.GetComponentInChildren<TextMeshPro>();
        if (tmp != null)
        {
            tmp.text = textValue;
            tmp.color = color;
        }
        else
        {
            Debug.LogWarning("Prefab does not contain a TextMeshPro component!");
        }

        // Add subtle sideways + upward force
        Rigidbody2D rb = textInstance.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float randomX = UnityEngine.Random.Range(-sidewaysMax, sidewaysMax); // much smaller sideways drift
            Vector2 direction = new Vector2(randomX, 1f).normalized;

            rb.AddForce(direction * upwardForce, ForceMode2D.Impulse);
            rb.AddTorque(UnityEngine.Random.Range(-torqueForce, torqueForce));
        }

        // Start fade-out coroutine
        MonoBehaviour runner = GetRunner();
        runner.StartCoroutine(FadeAndDestroy(tmp, textInstance, lifetime, fadeDuration));
    }

    // Fades text opacity and destroys the object
    private static IEnumerator FadeAndDestroy(TextMeshPro tmp, GameObject obj, float lifetime, float fadeDuration)
    {
        // Wait before starting fade
        yield return new WaitForSeconds(lifetime - fadeDuration);

        if (tmp != null)
        {
            Color startColor = tmp.color;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                float t = elapsed / fadeDuration;
                tmp.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(1f, 0f, t));
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        UnityEngine.Object.Destroy(obj);
    }

    /// <summary>
    /// Parses a string to a Color
    /// </summary>
    /// <param name="hex">The colors hex, make sure to add a #</param>
    /// <returns>The color deriverd from the string hex</returns>
    public static Color ColorFromHex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out var color);
        return color;
    }

    /// <summary>
    /// Adjusts a color by darkening or lightening it.
    /// </summary>
    /// <param name="color">The original color to adjust.</param>
    /// <param name="amount">
    /// Amount to adjust, from -1 to 1.  
    /// Positive = darken, negative = lighten.  
    /// 1 = full black, -1 = full white.
    /// </param>
    /// <returns>The adjusted color, with original alpha preserved.</returns>
    public static Color AdjustColorBrightness(Color color, float amount)
    {
        // Clamp amount to -1 -> 1 range
        amount = Mathf.Clamp(amount, -1f, 1f);

        float r = Mathf.Clamp01(color.r - amount);
        float g = Mathf.Clamp01(color.g - amount);
        float b = Mathf.Clamp01(color.b - amount);

        return new Color(r, g, b, color.a);
    }

    /// <summary>
    /// Adjusts the saturation of a color.
    /// </summary>
    /// <param name="color">The original color to adjust.</param>
    /// <param name="amount">
    /// Saturation adjustment amount (-1 to 1).  
    /// -1 = fully desaturated (gray), 0 = no change, 1 = maximum saturation boost.
    /// </param>
    /// <returns>The color with adjusted saturation, alpha preserved.</returns>
    public static Color AdjustColorSaturation(Color color, float amount)
    {
        // Clamp amount to -1..1
        amount = Mathf.Clamp(amount, -1f, 1f);

        // Compute luminance (grayscale)
        float gray = color.r * 0.299f + color.g * 0.587f + color.b * 0.114f;
        Color grayscale = new Color(gray, gray, gray, color.a);

        if (amount < 0f)
        {
            // Desaturate towards grayscale
            return Color.Lerp(color, grayscale, -amount);
        }
        else if (amount > 0f)
        {
            // Saturate: push colors away from grayscale
            // Simple formula: newColor = color + (color - gray) * amount
            float r = Mathf.Clamp01(color.r + (color.r - gray) * amount);
            float g = Mathf.Clamp01(color.g + (color.g - gray) * amount);
            float b = Mathf.Clamp01(color.b + (color.b - gray) * amount);
            return new Color(r, g, b, color.a);
        }
        else
        {
            return color; // No change
        }
    }

    public static IEnumerator FadeObject(GameObject obj, float start = 0, float end = 1, float duration = 0.25f)
    {
        if (obj == null) yield break;

        // Try to get components that can change opacity
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        Image image = obj.GetComponent<Image>();
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float alpha = Mathf.Lerp(start, end, t);

            // Apply alpha
            if (canvasGroup != null)
            {
                canvasGroup.alpha = alpha;
            }
            else if (image != null)
            {
                Color c = image.color;
                c.a = alpha;
                image.color = c;
            }
            else if (spriteRenderer != null)
            {
                Color c = spriteRenderer.color;
                c.a = alpha;
                spriteRenderer.color = c;
            }

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        // Ensure final value
        if (canvasGroup != null)
        {
            canvasGroup.alpha = end;
        }
        else if (image != null)
        {
            Color c = image.color;
            c.a = end;
            image.color = c;
        }
        else if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = end;
            spriteRenderer.color = c;
        }
    }

    public static IEnumerator EnableObjectDelay(GameObject obj, bool enable, float time)
    {
        yield return new WaitForSecondsRealtime(time);
        obj.SetActive(enable);
    }
    public static IEnumerator AnimateValue(float start, float end, float duration, AnimationCurve curve, Action<float> onValueChanged, bool useRealtime = false)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Choose between game time and real time
            elapsed += useRealtime ? Time.unscaledDeltaTime : Time.deltaTime;

            // Calculate t (0-1)
            float t = Mathf.Clamp01(elapsed / duration);

            // Apply animation curve
            float curvedT = curve.Evaluate(t);

            // Lerp value
            float value = Mathf.Lerp(start, end, curvedT);

            // Send value back
            onValueChanged?.Invoke(value);

            yield return null;
        }

        // Ensure final value
        onValueChanged?.Invoke(end);
    }

    // Creates a runner for coroutines (if one doesn't already exist)
    private static MonoBehaviour GetRunner()
    {
        if (_runner == null)
        {
            GameObject runnerGO = new GameObject("TextUtilityRunner");
            UnityEngine.Object.DontDestroyOnLoad(runnerGO);
            _runner = runnerGO.AddComponent<CoroutineRunner>();
        }
        return _runner;
    }
    private static CoroutineRunner _runner;

    private class CoroutineRunner : MonoBehaviour { }
}