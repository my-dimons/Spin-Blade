using UnityEngine;

public class DraggableSkillTreeMenu : MonoBehaviour
{
    [Header("Drag Settings")]
    public float edgePaddingX = 20f; // Minimum distance from left/right screen edge
    public float edgePaddingY = 20f; // Minimum distance from top/bottom screen edge

    private RectTransform rectTransform;
    private Vector2 offset;
    private Vector3 defaultPosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        defaultPosition = rectTransform.position;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            rectTransform.position = defaultPosition;
        }
        // Store offset when right-click starts
        if (Input.GetMouseButtonDown(1))
        {
            offset = (Vector2)rectTransform.position - (Vector2)Input.mousePosition;
        }

        // While holding right-click, drag and clamp
        if (Input.GetMouseButton(1))
        {
            Vector3 newPos = Input.mousePosition + (Vector3)offset;

            // Clamp position using padding
            rectTransform.position = ClampToScreen(newPos);
        }
    }

    private Vector3 ClampToScreen(Vector3 position)
    {
        // Get the size of the menu
        Vector2 size = rectTransform.sizeDelta * rectTransform.lossyScale;

        // Calculate clamped boundaries with adjustable padding
        float minX = (size.x / 2f) + edgePaddingX;
        float maxX = Screen.width - (size.x / 2f) - edgePaddingX;
        float minY = (size.y / 2f) + edgePaddingY;
        float maxY = Screen.height - (size.y / 2f) - edgePaddingY;

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);

        return position;
    }
}
