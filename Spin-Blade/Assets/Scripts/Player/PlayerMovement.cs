using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Waypoints")]
    public List<GameObject> waypoints;
    public GameObject currentWaypoint;
    public float minWaypointDistance = 0.05f;

    [Header("Movement")]
    public float speed;
    int direction = 1; // 1 = forward, -1 = backward
    public float rotateMultiplier;
    public bool canMove;

    public float spinSpeed;
    public GameObject sprite;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = waypoints[0].transform.position;
        currentWaypoint = waypoints[0];
    }

    // Update is called once per frame
    void Update()
    {
        sprite.transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
        // transition to the next waypoint if the player is close enough
        UpdateWaypoints();

        // Reversing input
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ReverseDirection();
        }
    }

    private void ReverseDirection()
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

    private void UpdateWaypoints()
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

    private void FixedUpdate()
    {
        // move player
        if (canMove)
            transform.position = Vector2.MoveTowards(transform.position, currentWaypoint.transform.position, speed * Time.deltaTime);
    }
}
