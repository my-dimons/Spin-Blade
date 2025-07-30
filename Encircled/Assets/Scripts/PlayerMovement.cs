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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = waypoints[0].transform.position;
        currentWaypoint = waypoints[0];
    }

    // Update is called once per frame
    void Update()
    {
        // transition to the next waypoint if the player is close enough
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

        if (Input.GetKeyDown(KeyCode.Space) && canMove)
        {
            canMove = false;
        } else if (Input.GetKeyUp(KeyCode.Space) && !canMove)
        {
            canMove = true;
        }

        // Reversing input
        if (Input.GetKeyDown(KeyCode.LeftShift))
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
    }


    private void FixedUpdate()
    {
        RotateTowardsTarget(currentWaypoint);

        // move player
        if (canMove)
            transform.position = Vector2.MoveTowards(transform.position, currentWaypoint.transform.position, speed * Time.deltaTime);

    }

    void RotateTowardsTarget(GameObject target)
    {
        Vector3 vectorToTarget = target.transform.position - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - rotateMultiplier;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * speed);
    }
}
