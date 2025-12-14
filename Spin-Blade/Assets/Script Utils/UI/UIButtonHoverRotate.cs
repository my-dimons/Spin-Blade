using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityUtils.ScriptUtils.UI
{
    [RequireComponent(typeof(Button))]
    public class UIButtonHoverRotate : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Adjustable Values")]
        /// When hovered this is the rotation the button will be set to.
        public float hoverRotation = 8f;

        /// The amount of seconds that the button will rotate in.
        public float rotationAnimationSeconds = 0.2f;

        /// If true the button will rotate to the set position, the rotate back on hover. If false the buton will rotate to the set position, then only rotate back when unhovering.
        public bool rotateBackAfterHover = true;

        [Space(8)]

        /// true to use unscaled real time for the animation (ignoring time scale).
        public bool useRealtime;

        /// The <see cref="AnimationCurve"/> that the button will follow.
        public AnimationCurve SizingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Random Rotation")]
        /// Not required, but if true rotation will use random rotation
        public bool useRandomRotation = false;

        /// The min/max values to use in random rotation
        public Vector2 randomRotation = new Vector2(-5f, 5f);
        private Vector3 currentRandomRotation;


        [Header("Debug Values")]

        /// True if the button is being hovered
        public bool hoveringOverButton;

        Vector3 originalRotation;
        Vector3 hoverRotationVector;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            originalRotation = new Vector3(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z);
        }

        // Update is called once per frame
        void Update()
        {
            hoverRotationVector = useRandomRotation ? currentRandomRotation : new Vector3(0, 0, hoverRotation);

            // Stops choppy animation when spam hovering the button
            bool stopChoppyAnimation = !hoveringOverButton && transform.localRotation == Quaternion.Euler(hoverRotationVector);
            bool rotateBackAfterHoverCondition = transform.localRotation == Quaternion.Euler(hoverRotationVector) && rotateBackAfterHover;

            if (stopChoppyAnimation || rotateBackAfterHoverCondition)
            {
                ExitHoverAnimation();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (transform.localRotation == Quaternion.Euler(originalRotation))
                EnterHoverAnimation();

            hoveringOverButton = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (transform.localRotation == Quaternion.Euler(hoverRotationVector))
                ExitHoverAnimation();

            hoveringOverButton = false;
        }

        /// <summary>
        /// Grows the button to the original size (<see cref="hoverRotationVector"/>).
        /// </summary>
        void EnterHoverAnimation()
        {
            currentRandomRotation = new Vector3(originalRotation.x, originalRotation.y, Random.Range(randomRotation.x, randomRotation.y));

            ObjectAnimations.AnimateTransformRotation(transform, originalRotation, useRandomRotation ? currentRandomRotation : hoverRotationVector, rotationAnimationSeconds, useRealtime, SizingCurve);
        }

        /// <summary>
        /// Shrinks the button to its original size.
        /// </summary>
        void ExitHoverAnimation()
        {
            ObjectAnimations.AnimateTransformRotation(transform, useRandomRotation ? currentRandomRotation : hoverRotationVector, originalRotation, rotationAnimationSeconds, useRealtime, SizingCurve);
        }
    }
}