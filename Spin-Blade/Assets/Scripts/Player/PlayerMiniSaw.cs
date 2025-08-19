using UnityEngine;

public class PlayerMiniSaw : MonoBehaviour
{
    [Header("Orbit Settings")]
    public Vector3 orbitPoint = Vector3.zero;  
    public float orbitRadius = 5f;          
    private float orbitAngle;
    public float speed = 90f;   
    private float actualRotationSpeed;
    private int direction = 1; // 1 = clockwise, -1 = counter-clockwise

    [Header("Visuals")]
    public GameObject sprite;
    public float spriteSpinSpeed = 180f;

    PlayerHealthAndDamage playerHealth;

    private void Start()
    {
        // pick a random starting angle
        float startAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(startAngle), Mathf.Sin(startAngle), 0) * orbitRadius;
        transform.position = orbitPoint + offset;

        // maybe reverse direction randomly
        if (Random.value > 0.5f)
        {
            ReverseHexagonDirection();
        }

        // set up speed
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthAndDamage>();
        IncreaseSpeed(playerHealth.miniSawBaseSpeed);
    }

    private void Update()
    {
        // spin sprite
        sprite.transform.Rotate(0, 0, spriteSpinSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        // Update angle
        orbitAngle += speed * direction * Time.deltaTime;

        // Calculate new position
        float x = orbitPoint.x + Mathf.Cos(orbitAngle) * orbitRadius;
        float y = orbitPoint.y + Mathf.Sin(orbitAngle) * orbitRadius;

        // Apply position (no rotation)
        transform.position = new Vector3(x, y, transform.position.z);
    }

    public void IncreaseSpeed(float amount)
    {
        speed += amount;
        float variance = speed / 5f;
        actualRotationSpeed = speed + Random.Range(-variance, variance);
    }

    public void ReverseHexagonDirection()
    {
        Debug.Log("Reversing Saw Direction");
        direction *= -1;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow; // Circle color
        Gizmos.DrawWireSphere(orbitPoint, orbitRadius); // Draw orbit circle
    }
}
