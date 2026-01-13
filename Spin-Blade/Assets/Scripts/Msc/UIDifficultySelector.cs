using UnityEngine;
using UnityEngine.EventSystems;
using UnityUtils.ScriptUtils.Objects;

public class UIDifficultySelector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Difficulty Popup Settings")]
    public GameObject difficultyPopup;
    [Space(10)]
    public Vector3 popupShrunkSize = Vector3.one * 0.8f;
    public Vector3 popupGrownSize = Vector3.one;
    [Space(10)]
    public float popupAnimationDuration = 0.2f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        difficultyPopup.SetActive(true);
        ObjectAnimations.AnimateTransformScale(difficultyPopup.transform,
            popupShrunkSize,
            popupGrownSize,
            popupAnimationDuration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ObjectAnimations.AnimateTransformScale(difficultyPopup.transform,
            popupGrownSize,
            popupShrunkSize,
            popupAnimationDuration);

        ObjectDelays.CallFunctionAfterTime(
            () => { difficultyPopup.SetActive(false); },
            popupAnimationDuration);
    }
}
