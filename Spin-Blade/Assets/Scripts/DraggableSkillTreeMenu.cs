using UnityEngine;

public class DraggableSkillTreeMenu : MonoBehaviour
{
    [Header("Drag Settings")]
    public float maxXLimit = 500f;
    public float maxYLimit = 300f;

    [Header("Zoom Settings")]
    public Transform zoomParent; // Parent object to scale
    public float zoomStep = 0.1f;
    public float maxZoomIn = 2f;
    public float maxZoomOut = 0.5f;

    private RectTransform rectTransform;
    private Vector3 defaultPosition;
    private Vector3 offset;

    private Canvas parentCanvas;
    private Camera canvasCamera;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        canvasCamera = parentCanvas.worldCamera;

        defaultPosition = rectTransform.localPosition;
        Debug.Log(defaultPosition);

        if (zoomParent == null)
        {
            Debug.LogWarning("Zoom parent not assigned! Please set it in the Inspector.");
        }
    }

    void Update()
    {
        HandleDrag();
        if (!Input.GetMouseButton(1))
            HandleZoom();

        // Reset position & zoom
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPosition();
        }
    }

    private void HandleDrag()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPos = ScreenToWorldPoint(Input.mousePosition);
            offset = rectTransform.position - mouseWorldPos;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 mouseWorldPos = ScreenToWorldPoint(Input.mousePosition);
            Vector3 newPos = mouseWorldPos + offset;
            rectTransform.position = ClampToBounds(newPos);
        }
    }

    private void HandleZoom()
    {
        if (zoomParent == null) return;

        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Approximately(scroll, 0f)) return;

        // Calculate new scale
        float newScale = Mathf.Clamp(
            zoomParent.localScale.x + (scroll > 0 ? zoomStep : -zoomStep),
            maxZoomOut,
            maxZoomIn
        );

        zoomParent.localScale = Vector3.one * newScale;
    }

    private Vector3 ClampToBounds(Vector3 position)
    {
        // Convert defaultPosition into world position for clamping
        Vector3 worldDefaultPos = rectTransform.parent.TransformPoint(defaultPosition);

        position.x = Mathf.Clamp(position.x, worldDefaultPos.x - maxXLimit, worldDefaultPos.x + maxXLimit);
        position.y = Mathf.Clamp(position.y, worldDefaultPos.y - maxYLimit, worldDefaultPos.y + maxYLimit);
        return position;
    }

    private Vector3 ScreenToWorldPoint(Vector3 screenPos)
    {
        return canvasCamera != null
            ? canvasCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, canvasCamera.nearClipPlane))
            : screenPos;
    }

    public void ResetPosition()
    {
        rectTransform.localPosition = defaultPosition;

        if (zoomParent != null)
            zoomParent.localScale = Vector3.one;
    }
}
