using UnityEngine;

public class RotateTowardsObject : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;       
    Vector3 targetPosition;
    public float rotationOffset = 0f;
    public float altRotationOffset = 0f;
    public bool useAltRotationOffset = false;

    void Update()
    {
        if (target == null) targetPosition = Vector3.zero;
        targetPosition = target.position;

        Vector3 direction = targetPosition - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (useAltRotationOffset)
            angle += altRotationOffset;
        else
            angle += rotationOffset;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
