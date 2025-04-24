using System.Collections;
using UnityEngine;
public class WaypointMover : MonoBehaviour
{
    public Transform waypointParent;
    public float moveSpeed = 2f;
    public float waitTime = 2f;
    public bool loopWaypoints = true;

    private Transform[] waypoints;
    private int currentWaypointIndex;
    private bool isWaiting;
    private Animator animator;
    private float lastInputX;
    private float lastInputY;

    private bool isReversing = false;
    private bool shouldDisappearOnArrival = false;

    // Add a property to get the current waypoint index
    public int CurrentWaypointIndex => currentWaypointIndex;

    void Start()
    {
        animator = GetComponent<Animator>();
        waypoints = new Transform[waypointParent.childCount];
        for (int i = 0; i < waypointParent.childCount; i++)
        {
            waypoints[i] = waypointParent.GetChild(i);
        }
    }

    void Update()
    {
        if (PauseController.IsGamePaused || isWaiting)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputX", lastInputX);
            animator.SetFloat("LastInputY", lastInputY);
            return;
        }
        MoveToWaypoint();
    }

    public void StartReverseMovement()
    {
        isReversing = true;
        shouldDisappearOnArrival = true;
        currentWaypointIndex = waypoints.Length - 1; // Start from last waypoint
        isWaiting = false; // Immediately start moving
    }

    void MoveToWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypointIndex];
        Vector2 direction = (target.position - transform.position).normalized;

        if (direction.magnitude > 0f)
        {
            lastInputX = direction.x;
            lastInputY = direction.y;
        }
        transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        animator.SetFloat("InputX", direction.x);
        animator.SetFloat("InputY", direction.y);
        animator.SetBool("isWalking", direction.magnitude > 0f);
        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            StartCoroutine(WaitAtWaypoint());
        }
    }

    IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        animator.SetBool("isWalking", false);
        animator.SetFloat("LastInputX", lastInputX);
        animator.SetFloat("LastInputY", lastInputY);
        yield return new WaitForSeconds(waitTime);

        if (isReversing)
        {
            // Move to previous waypoint when reversing
            if (currentWaypointIndex > 0)
            {
                currentWaypointIndex--;
            }
            else if (shouldDisappearOnArrival)
            {
                // Reached first waypoint - disappear
                gameObject.SetActive(false);
                yield break; // Exit coroutine early
            }
        }
        else
        {
            //If looping is enabled: increment currentWaypointIndex and wrap around if needed
            //If not looping: increment currentWaypointIndex but dont exceed last waypoint
            currentWaypointIndex = loopWaypoints ? (currentWaypointIndex + 1) % waypoints.Length : Mathf.Min(currentWaypointIndex + 1, waypoints.Length - 1);
        }
        isWaiting = false;
    }

    // Check if the NPC is currently moving
    public bool IsMoving()
    {
        return !isWaiting && !PauseController.IsGamePaused;
    }

    // Check if we're at the last waypoint index
    public bool IsAtLastWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0)
            return false;

        return currentWaypointIndex == waypoints.Length - 1;
    }

    // Get the total number of waypoints
    public int WaypointCount()
    {
        return waypoints != null ? waypoints.Length : 0;
    }

    public bool IsWaiting()
    {
        return isWaiting;
    }

    public bool IsReversing()
    {
        return isReversing;
    }
}