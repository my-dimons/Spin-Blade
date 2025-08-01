using UnityEngine;

public class DraggableSkillTreeMenu : MonoBehaviour
{
    [Header("Drag Settings")]
    public float maxXLimit = 500f; // Max distance you can drag left/right
    public float maxYLimit = 300f; // Max distance you can drag up/down

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

        defaultPosition = rectTransform.position;
    }

    void Update()
    {
        // Start dragging with right-click
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPos = ScreenToWorldPoint(Input.mousePosition);
            offset = rectTransform.position - mouseWorldPos;
        }

        // Dragging
        if (Input.GetMouseButton(1))
        {
            Vector3 mouseWorldPos = ScreenToWorldPoint(Input.mousePosition);
            Vector3 newPos = mouseWorldPos + offset;
            rectTransform.position = ClampToBounds(newPos);
        }

        // Reset position
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPosition();
        }
    }

    private Vector3 ClampToBounds(Vector3 position)
    {
        // Clamp X and Y relative to the default position
        position.x = Mathf.Clamp(position.x, defaultPosition.x - maxXLimit, defaultPosition.x + maxXLimit);
        position.y = Mathf.Clamp(position.y, defaultPosition.y - maxYLimit, defaultPosition.y + maxYLimit);
        return position;
    }

    private Vector3 ScreenToWorldPoint(Vector3 screenPos)
    {
        // Convert mouse screen position into world position for the canvas camera
        return canvasCamera != null
            ? canvasCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, canvasCamera.nearClipPlane))
            : screenPos;
    }

    public void ResetPosition()
    {
        rectTransform.position = defaultPosition;
    }
}