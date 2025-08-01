using System.Collections;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    Vector3 pos;

    public AnimationCurve curve;
    public bool screenshaking;

    private void Start()
    {
        pos = transform.localPosition;
    }

    public void ScreenshakeFunction(float duration = .5f)
    {
        StartCoroutine(Screenshake(duration));
    }
    public IEnumerator Screenshake(float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float strength = curve.Evaluate(elapsedTime / duration);
            transform.localPosition = pos + Random.insideUnitSphere * strength;
            yield return null;
        }
        transform.localPosition = pos;
    }
}
