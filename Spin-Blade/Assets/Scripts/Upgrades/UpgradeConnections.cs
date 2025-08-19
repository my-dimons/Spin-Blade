using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UI.Extensions; // UILineRenderer namespace

[RequireComponent(typeof(Upgrade))]
public class UpgradeConnections : MonoBehaviour
{
    [Header("Line Settings")]
    public Color connectorDisabledColor;
    public Color connectorDisabledColorMaxed;
    public Color connectorEnabledColor;
    [Space(8)]
    public float lineThickness = 5f;
    [Space(8)]
    public GameObject linePrefab;
    private GameObject[] skillTreePrecursors;

    public List<GameObject> lineObjects = new();
    public List<UILineRenderer> lineRenderers = new();

    private Upgrade upgrade;

    /*
    private void OnValidate()
    {
        Initialize();

        if (lineObjects.Count <= 0)
            CreateLines();
        else
            UpdateConnecters();
    }
    */

    void Start()
    {
        Initialize();

        CreateLines();
    }

    private void Initialize()
    {
        if (upgrade == null)
            upgrade = GetComponent<Upgrade>();
        skillTreePrecursors = upgrade.skillTreePrecursors;
    }

    void Update() 
    {
        if (skillTreePrecursors == null || skillTreePrecursors.Length == 0)
            return;

        UpdateConnecters();
    }

    private void UpdateConnecters()
    {
        for (int i = 0; i < skillTreePrecursors.Length; i++)
        {
            // set connector color
            UILineRenderer connectorRenderer = lineRenderers[i];
            GameObject precursor = skillTreePrecursors[i];

            UpdateConnecterColor(connectorRenderer);

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

    private void UpdateConnecterColor(UILineRenderer connectorRenderer)
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
            DestroyImmediate(lineObj);

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

    private void OnDrawGizmos()
    {
        float gizmoLineWidth = 0.3f;
        if (upgrade == null)
            upgrade = GetComponent<Upgrade>();

        if (upgrade == null || upgrade.skillTreePrecursors == null)
            return;

        foreach (var precursor in upgrade.skillTreePrecursors)
        {
            if (precursor == null) continue;

            Vector3 start = upgrade.buyButton.transform.position;
            Vector3 end = precursor.GetComponent<Upgrade>().buyButton.transform.position;
            Vector3 dir = end - start;
            float length = dir.magnitude;

            if (length > 0.0001f)
            {
                Vector3 mid = (start + end) / 2f;

                // Build a rotation that points the cube's "up" (Y axis) along dir
                Quaternion rot = Quaternion.FromToRotation(Vector3.up, dir.normalized);

                Gizmos.color = Color.green;
                Gizmos.matrix = Matrix4x4.TRS(mid, rot, new Vector3(gizmoLineWidth, length, gizmoLineWidth));
                Gizmos.DrawCube(Vector3.zero, Vector3.one); // draw unit cube with transform matrix
                Gizmos.matrix = Matrix4x4.identity; // reset so we don't affect other gizmos
            }
        }
    }
}
