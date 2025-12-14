using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityUtils.ScriptUtils.UI
{
    [RequireComponent(typeof(Button))]
    public class UIButtonHoverExpand : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Adjustable Values")]
        /// When hovered this is the size the button will be set to.
        public float hoverSize = 1.3f;

        /// The amount of seconds that the button will size up or down in.
        public float sizeAnimationSeconds = 0.2f;

        [Space(8)]

        /// true to use unscaled real time for the animation (ignoring time scale)
        public bool useRealtime;

        /// The <see cref="AnimationCurve"/> that the button will follow.
        public AnimationCurve SizingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Debug Values")]

        /// True if the button is being hovered
        public bool hoveringOverButton;

        Vector3 originalSize;
        Vector3 hoverSizeVector;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            originalSize = transform.localScale;
        }

        // Update is called once per frame
        void Update()
        {
            hoverSizeVector = new Vector3(hoverSize, hoverSize, hoverSize);

            // Stops choppy animation when spam hovering the button
            if (!hoveringOverButton && transform.localScale == hoverSizeVector)
            {
                ExitHoverAnimation();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (transform.localScale == originalSize)
                EnterHoverAnimation();

            hoveringOverButton = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (transform.localScale == hoverSizeVector)
                ExitHoverAnimation();

            hoveringOverButton = false;
        }

        /// <summary>
        /// Grows the button to the original size (<see cref="hoverSizeVector"/>).
        /// </summary>
        void EnterHoverAnimation()
        {
            ObjectAnimations.AnimateTransformScale(transform, originalSize, hoverSizeVector, sizeAnimationSeconds, useRealtime, SizingCurve);
        }

        /// <summary>
        /// Shrinks the button to its original size.
        /// </summary>
        void ExitHoverAnimation()
        {
            ObjectAnimations.AnimateTransformScale(transform, hoverSizeVector, originalSize, sizeAnimationSeconds, useRealtime, SizingCurve);
        }
    }
}
