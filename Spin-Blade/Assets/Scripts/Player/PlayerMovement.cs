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
    public enum MovementParticleColorEnum
    {
        Normal,
        Win
    }
    public MovementParticleColorEnum particleSystemEnum = MovementParticleColorEnum.Normal;
    private Color movementParticleColor;
    private float movementParticleAlpha = 0.2f;

    [Header("Audio")]
    public AudioClip reverseDirectionSound;

    private int direction = 1; // 1 = clockwise, -1 = counter-clockwise
    public bool switchKey;

    private void OnValidate()
    {
        SetMovementParticleColor();
    }
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

        SetMovementParticleColor();
    }

    private void SetMovementParticleColor()
    {
        switch (particleSystemEnum)
        {
            case MovementParticleColorEnum.Normal:
                movementParticleColor = new Color(0.3607843f, 0.764706f, 1f, movementParticleAlpha);
                break;
            case MovementParticleColorEnum.Win:
                movementParticleColor = new Color(1f, 0.7843137f, 0.3058824f, movementParticleAlpha);
                break;
        }

        ParticleSystem.MainModule par = movementParticles.main;
        par.startColor = movementParticleColor;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow; // Circle color
        Gizmos.DrawWireSphere(orbitPoint, orbitRadius); // Draw orbit circle
    }
}
