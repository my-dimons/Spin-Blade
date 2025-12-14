using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UnityUtils.ScriptUtils
{
    public static class ObjectAnimations
    {
        #region This region is assisted by ChatGPT
        // Had to somehow have it start coroutines without someone manually assigning a script
        private class ObjectAnimationsCoroutineStarter : MonoBehaviour { }
        private static ObjectAnimationsCoroutineStarter coroutineStarter;
        private static ObjectAnimationsCoroutineStarter CoroutineStarter
        {
            get
            {
                if (coroutineStarter == null)
                {
                    GameObject starter = new GameObject("Unity Utils - Coroutine Starter");
                    coroutineStarter = starter.AddComponent<ObjectAnimationsCoroutineStarter>();
                    UnityEngine.Object.DontDestroyOnLoad(starter);
                }

                return coroutineStarter;
            }
        }
        #endregion

        /// <summary>
        /// Animates an objects <see cref="Transform"/> components scale from a starting value to an ending value over a specified duration.
        /// </summary>
        public static void AnimateTransformScale(Transform transform, Vector3 startScale, Vector3 endScale, float duration, bool useRealtime = false, AnimationCurve animationCurve = default)
        {
            if (animationCurve == default) animationCurve = AnimationCurve.Linear(0, 0, 1, 1);

            AnimateValue<Vector3>(startScale, endScale, duration, (a, b, t) => Vector3.Lerp(a, b, t), value => transform.localScale = value, useRealtime, animationCurve);
        }

        /// <summary>
        /// Animates an objects <see cref="Transform"/> components position from a starting value to an ending value over a specified duration.
        /// </summary>
        public static void AnimateTransformPosition(Transform transform, Vector3 startPos, Vector3 endPos, float duration, bool useRealtime = false, AnimationCurve animationCurve = default)
        {
            if (animationCurve == default) animationCurve = AnimationCurve.Linear(0, 0, 1, 1);

            AnimateValue<Vector3>(startPos, endPos, duration, (a, b, t) => Vector3.Lerp(a, b, t), value => transform.position = value, useRealtime, animationCurve);
        }

        /// <summary>
        /// Animates an objects <see cref="Transform"/> components rotation from a starting value to an ending value over a specified duration.
        /// </summary>
        public static void AnimateTransformRotation(Transform transform, Vector3 startRotation, Vector3 endRotation, float duration, bool useRealtime = false, AnimationCurve animationCurve = default)
        {
            if (animationCurve == default) animationCurve = AnimationCurve.Linear(0, 0, 1, 1);

            AnimateValue<Vector3>(startRotation, endRotation, duration, (a, b, t) => Vector3.Lerp(a, b, t), value => transform.localRotation = Quaternion.Euler(value), useRealtime, animationCurve);
        }

        /// <summary>
        /// Animates an objects <see cref="SpriteRenderer"/> component alpha from a starting value to an ending value over a specified duration.
        /// </summary>
        public static void AnimateSpriteRendererOpacity(SpriteRenderer spriteRenderer, float startOpacity, float endOpacity, float duration, bool useRealtime = false, AnimationCurve animationCurve = default)
        {
            if (animationCurve == default) animationCurve = AnimationCurve.Linear(0, 0, 1, 1);

            Color color = spriteRenderer.color;

            AnimateValue<float>(startOpacity, endOpacity, duration, (a, b, t) => Mathf.Lerp(a, b, t), value => spriteRenderer.color = new Color(color.r, color.g, color.b, value), useRealtime, animationCurve);
        }

        /// <summary>
        /// Animates an objects <see cref="Image"/> component alpha from a starting value to an ending value over a specified duration.
        /// </summary>
        public static void AnimateImageOpacity(Image image, float startOpacity, float endOpacity, float duration, bool useRealtime = false, AnimationCurve animationCurve = default)
        {
            if (animationCurve == default) animationCurve = AnimationCurve.Linear(0, 0, 1, 1);

            Color color = image.color;

            AnimateValue<float>(startOpacity, endOpacity, duration, (a, b, t) => Mathf.Lerp(a, b, t), value => image.color = new Color(color.r, color.g, color.b, value), useRealtime, animationCurve);
        }

        /// <summary>
        /// Animates a value from a starting value to an ending value over a specified duration
        /// </summary>
        /// <param name="lerpFunction">Decides how the start and end are lerped. Use things like Mathf.Lerp() or Vector3.Lerp() for different types of values</param>
        /// <param name="onValueChanged">A callback that is invoked with the current interpolated Vector3 value as the animation progresses.</param>
        /// <param name="useRealtime">true to use unscaled real time for the animation (ignoring time scale)</param>
        /// <param name="animationCurve">Default is a linear curve</param>
        public static void AnimateValue<T>(T start, T end, float duration, Func<T, T, float, T> lerpFunction, Action<T> onValueChanged, bool useRealtime = false, AnimationCurve curve = default)
        {
            if (curve == default) curve = AnimationCurve.Linear(0, 0, 1, 1);
            CoroutineStarter.StartCoroutine(AnimateValueCoroutine(start, end, curve, duration, lerpFunction, onValueChanged, useRealtime));
        }

        private static IEnumerator AnimateValueCoroutine<T>(T start, T end, AnimationCurve curve, float duration, Func<T, T, float, T> lerpFunction, Action<T> onValueChanged, bool useRealtime = false)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += useRealtime ? Time.unscaledDeltaTime : Time.deltaTime;

                float time = Mathf.Clamp01(elapsed / duration);

                T value = lerpFunction(start, end, curve.Evaluate(time));

                onValueChanged?.Invoke(value);

                yield return null;
            }

            // Ensure final value
            onValueChanged?.Invoke(end);
        }
    }
}