using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions; // UILineRenderer namespace

public class UpgradeConnections : MonoBehaviour
{
    [Header("Colors")]
    public Color connectorDisabledColor;
    public Color connectorDisabledColorMaxed;
    public Color connectorEnabledColor;

    public GameObject[] skillTreePrecursors;
    public GameObject linePrefab;

    public float lineThickness = 5f;

    private List<GameObject> lineObjects = new();
    private List<UILineRenderer> lineRenderers = new();

    private Upgrade upgrade;

    private void OnValidate()
    {
        if (lineObjects.Count <= 0)
        {
            //CreateLines();
        }
    }

    void Start()
    {
        upgrade = GetComponent<Upgrade>();
        skillTreePrecursors = upgrade.skillTreePrecursors;

        CreateLines();
    }



    void Update()
    {
        if (skillTreePrecursors == null || skillTreePrecursors.Length == 0)
            return;

        UpdatePrecursorConnecters();
    }

    private void UpdatePrecursorConnecters()
    {
        for (int i = 0; i < skillTreePrecursors.Length; i++)
        {
            // set connector color
            UILineRenderer connectorRenderer = lineRenderers[i];
            GameObject precursor = skillTreePrecursors[i];

            UpdateConnecterColors(connectorRenderer);

            UpdateConnectorPoints(precursor, connectorRenderer);
        }
    }

    private void UpdateConnectorPoints(GameObject precursor, UILineRenderer lineRenderer)
    {
        // Get RectTransforms for buyButtons of current and precursor
        RectTransform startRect = upgrade.buyButton.GetComponent<RectTransform>();
        RectTransform endRect = precursor.GetComponent<Upgrade>().buyButton.GetComponent<RectTransform>();

        // Convert world positions to local positions relative to the parent canvas or line's RectTransform

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            lineRenderer.rectTransform.parent as RectTransform,
            RectTransformUtility.WorldToScreenPoint(null, startRect.position),
            null,
            out Vector2 localStartPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            lineRenderer.rectTransform.parent as RectTransform,
            RectTransformUtility.WorldToScreenPoint(null, endRect.position),
            null,
            out Vector2 localEndPos);

        // Update points of UILineRenderer
        lineRenderer.Points = new Vector2[] { localStartPos, localEndPos };
    }

    private void UpdateConnecterColors(UILineRenderer connectorRenderer)
    {
        if (upgrade.canBeBought)
            connectorRenderer.color = connectorEnabledColor;
        else if (!upgrade.canBeBought && upgrade.precursorsMustBeMaxxed)
            connectorRenderer.color = connectorDisabledColorMaxed;
        else
            connectorRenderer.color = connectorDisabledColor;
    }

    void CreateLines()
    {
        // Clear old lines
        foreach (var lineObj in lineObjects)
            Destroy(lineObj);

        lineObjects.Clear();
        lineRenderers.Clear();

        foreach (var precursor in skillTreePrecursors)
        {
            if (precursor == null) continue;

            GameObject lineObj = Instantiate(linePrefab, transform.parent);
            lineObj.transform.SetAsFirstSibling();
            
            if (!lineObj.TryGetComponent<UILineRenderer>(out var lr))
            {
                Debug.LogError("Line prefab missing UILineRenderer component!");
                Destroy(lineObj);
                continue;
            }

            lr.LineThickness = lineThickness;
            lr.color = connectorEnabledColor;

            lineObjects.Add(lineObj);
            lineRenderers.Add(lr);
        }
    }
}
