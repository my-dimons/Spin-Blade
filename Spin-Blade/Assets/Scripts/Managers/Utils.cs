using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class Utils
{
    public static void PlayClip(AudioClip clip, float volume = 1f, float pitchVariance = 0.1f)
    { 
        if (clip == null) return;

        GameObject tempGO = new GameObject("TempAudio");
        if (Camera.main != null)
            tempGO.transform.position = Camera.main.transform.position;

        // Get volume multiplier from GameManager
        GameManager gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        // Set up AudioSource
        AudioSource audioSource = tempGO.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume * gm.sfxVolume;

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