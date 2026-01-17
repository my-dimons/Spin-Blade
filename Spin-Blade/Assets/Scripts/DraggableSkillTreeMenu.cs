using UnityEngine;

public class DraggableSkillTreeMenu : MonoBehaviour
{
    [Header("Drag Settings")]
    public float maxXLimit = 500f;
    public float maxYLimit = 300f;

    [Header("Zoom Settings")]
    public Transform zoomParent; // Parent object to scale
    public float mobileZoomMultiplier = 1;
    public float zoomStep = 0.1f;
    public float maxZoomIn = 2f;
    public float maxZoomOut = 0.5f;

    private bool isDragging = false;

    private RectTransform rectTransform;
    private Vector3 defaultPosition;
    private Vector3 offset;

    private Canvas parentCanvas;
    private Camera canvasCamera;

    private MoneyManager moneyManager;

    private void Start()
    {
        moneyManager = MoneyManager.Instance;
    }

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        canvasCamera = parentCanvas.worldCamera;

        defaultPosition = rectTransform.localPosition;

        if (zoomParent == null)
        {
            Debug.LogWarning("Zoom parent not assigned! Please set it in the Inspector.");
        }
    }

    void Update()
    {
        HandleDrag();
        HandleZoom();

        // Reset position & zoom
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPosition();
        }
    }

    /// <summary>
    /// Drags the screen if holding left or right mouse button, unless hovering over shop element with left mouse button.
    /// </summary>
    private void HandleDrag()
    {
        // Initiate drag
        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && !moneyManager.hoveringOverShopElement)
        {
            isDragging = true;
            Vector3 mouseWorldPos = ScreenToWorldPoint(Input.mousePosition);
            offset = rectTransform.position - mouseWorldPos;
        }

        // Continue drag
        else if (isDragging && (Input.GetMouseButton(0) || Input.GetMouseButton(1)))
        {
            Vector3 mouseWorldPos = ScreenToWorldPoint(Input.mousePosition);
            Vector3 newPos = mouseWorldPos + offset;
            rectTransform.position = ClampToBounds(newPos);
        }

        // End drag
        else if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            isDragging = false;
        }
    }

    private void HandleZoom()
    {
        if (zoomParent == null) return;

        float scroll = Input.touchCount == 2 ? GetTouchZoomAmount() : Input.mouseScrollDelta.y;
        if (Mathf.Approximately(scroll, 0f)) return;

        // Calculate new scale
        float newScale = Mathf.Clamp(
            zoomParent.localScale.x + (scroll > 0 ? zoomStep : -zoomStep),
            maxZoomOut,
            maxZoomIn
        );

        zoomParent.localScale = Vector3.one * newScale;
    }

    private float GetTouchZoomAmount()
    {
        Touch touch0 = Input.GetTouch(0);
        Touch touch1 = Input.GetTouch(1);

        float previousDistance = (touch0.deltaPosition - touch1.deltaPosition).magnitude;
        float currentDistance = (touch0.position - touch1.position).magnitude;

        return (currentDistance - previousDistance) * mobileZoomMultiplier;
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
