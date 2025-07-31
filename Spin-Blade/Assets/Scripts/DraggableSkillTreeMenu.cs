using UnityEngine;

public class DraggableSkillTreeMenu : MonoBehaviour
{
    [Header("Drag Settings")]
    public float maxXLimit = 500f; // Max distance you can drag left/right
    public float maxYLimit = 300f; // Max distance you can drag up/down

    private RectTransform rectTransform;
    private Vector2 offset;
    private Vector3 defaultPosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        defaultPosition = rectTransform.position; // Store initial position
    }

    void Update()
    {
        // Start dragging with right-click
        if (Input.GetMouseButtonDown(1))
        {
            offset = (Vector2)rectTransform.position - (Vector2)Input.mousePosition;
        }

        // Dragging
        if (Input.GetMouseButton(1))
        {
            Vector3 newPos = Input.mousePosition + (Vector3)offset;
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
        // Use maxXLimit and maxYLimit, automatically applying the negative min
        position.x = Mathf.Clamp(position.x, -maxXLimit, maxXLimit);
        position.y = Mathf.Clamp(position.y, -maxYLimit, maxYLimit);
        return position;
    }

    public void ResetPosition()
    {
        rectTransform.position = defaultPosition;
    }
}
