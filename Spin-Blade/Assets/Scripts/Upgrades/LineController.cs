using UnityEngine;
using UnityEngine.UI.Extensions;

public class LineController : MonoBehaviour
{
    private UILineRenderer uiLineRenderer;
    private RectTransform parentRect;

    // The world-space points to connect
    public Transform[] points;

    private void Awake()
    {
        uiLineRenderer = GetComponent<UILineRenderer>();
        parentRect = uiLineRenderer.rectTransform.parent as RectTransform;
    }

    private void Update()
    {
        if (points == null || points.Length == 0)
            return;

        Vector2[] localPoints = new Vector2[points.Length];

        for (int i = 0; i < points.Length; i++)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                RectTransformUtility.WorldToScreenPoint(null, points[i].position),
                null,
                out Vector2 localPos);

            localPoints[i] = localPos;
        }

        uiLineRenderer.Points = localPoints;
    }
}
