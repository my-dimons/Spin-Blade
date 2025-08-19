using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Orbit Settings")]
    public Vector3 orbitPoint = Vector3.zero; 
    public float orbitRadius = 5f;          
    private float orbitAngle;
    public float speed = 90f;          
    public bool canMove = true;

    [Header("Sprite + FX")]
    public float spinSpeed;
    public GameObject sprite;
    public ParticleSystem movementParticles;

    [Header("Audio")]
    public AudioClip reverseDirectionSound;

    private int direction = 1; // 1 = clockwise, -1 = counter-clockwise
    public bool switchKey;

    void Start()
    {
        // Ensure player starts at correct distance from orbitPoint
        Vector3 offset = (transform.position - orbitPoint).normalized * orbitRadius;
        transform.position = orbitPoint + offset;
    }

    void Update()
    {
        // Input
        switchKey = Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.Space);
        // spin sprite
        sprite.transform.Rotate(0, 0, spinSpeed * Time.deltaTime);

        if (switchKey && Time.timeScale > 0)
        {
            ReverseDirection();
        }
    }


    private void FixedUpdate()
    {
        if (!canMove) return;

        // Update angle
        orbitAngle += speed * direction * Time.deltaTime;

        // Calculate new position
        float x = orbitPoint.x + Mathf.Cos(orbitAngle) * orbitRadius;
        float y = orbitPoint.y + Mathf.Sin(orbitAngle) * orbitRadius;

        // Apply position (no rotation)
        transform.position = new Vector3(x, y, transform.position.z);
    }

    private void ReverseDirection()
    {
        Utils.PlayAudioClip(reverseDirectionSound, 0.06f);
        direction *= -1;

        // Flip particle system direction
        var shape = movementParticles.shape;
        Vector3 currentScale = shape.scale;
        currentScale.z = -currentScale.z;
        shape.scale = currentScale;

        RotateTowardsObject partObj = movementParticles.gameObject.GetComponent<RotateTowardsObject>();
        partObj.useAltRotationOffset = !partObj.useAltRotationOffset;

        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().lShiftPresses++;
    }
}
