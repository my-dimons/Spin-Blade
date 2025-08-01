using System.Collections.Generic;
using UnityEngine;

public class PlayerMiniSaw : MonoBehaviour
{
    // waypoints
    public List<GameObject> waypoints;
    public GameObject currentWaypoint;
    public float minWaypointDistance = 0.05f;

    PlayerHealth playerHealth;
    int direction = 1; // 1 = forward, -1 = backward
    public GameObject sprite;
    public float rotationSpeed;
    
    private void Start()
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }
    private void Update()
    {
        sprite.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        UpdateHexagonWaypoints();
    }
    private void UpdateHexagonWaypoints()
    {
        if (Vector2.Distance(transform.position, currentWaypoint.transform.position) < minWaypointDistance)
        {
            if (Vector2.Distance(transform.position, currentWaypoint.transform.position) < minWaypointDistance)
            {
                int currentIndex = waypoints.IndexOf(currentWaypoint);
                int nextIndex = currentIndex + direction;

                // Check bounds
                if (nextIndex >= waypoints.Count)
                {
                    nextIndex = 0; // Loop forward
                }
                else if (nextIndex < 0)
                {
                    nextIndex = waypoints.Count - 1; // Loop backward
                }

                currentWaypoint = waypoints[nextIndex];
            }
        }
    }
    public void ReverseHexagonDirection()
    {
        direction *= -1; // Flip direction immediately

        // Move to the previous waypoint if we just reversed
        int currentIndex = waypoints.IndexOf(currentWaypoint);
        int nextIndex = currentIndex + direction;

        // Bounds check again (optional but smooth)
        if (nextIndex >= waypoints.Count) nextIndex = 0;
        if (nextIndex < 0) nextIndex = waypoints.Count - 1;

        currentWaypoint = waypoints[nextIndex];
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Circle"))
        {
            ReverseHexagonDirection();
        }
    }

    private void FixedUpdate()
    {
        // move hexagon
        transform.position = Vector2.MoveTowards(transform.position, currentWaypoint.transform.position, playerHealth.miniSawSpeed * Time.deltaTime);
    }
}
