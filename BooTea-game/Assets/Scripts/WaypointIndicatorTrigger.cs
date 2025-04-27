using UnityEngine;

[RequireComponent(typeof(WaypointMover))]
[RequireComponent(typeof(NPCIndicatorController))]
public class WaypointIndicatorTrigger : MonoBehaviour
{
    [Tooltip("Distance threshold to consider NPC at a waypoint")]
    public float waypointReachedThreshold = 0.2f; // Slightly increased threshold

    private WaypointMover waypointMover;
    private NPCIndicatorController indicatorController;
    private Transform[] waypoints;
    private bool indicatorCurrentlyShowing = false;
    private bool hasStoppedPermanently = false;
    private Transform lastWaypoint;
    private float movementThreshold = 0.01f; // Threshold to detect movement

    private Vector3 lastPosition; // To track movement

    private void Start()
    {
        waypointMover = GetComponent<WaypointMover>();
        indicatorController = GetComponent<NPCIndicatorController>();
        lastPosition = transform.position;

        if (waypointMover.waypointParent != null)
        {
            waypoints = new Transform[waypointMover.waypointParent.childCount];
            for (int i = 0; i < waypointMover.waypointParent.childCount; i++)
            {
                waypoints[i] = waypointMover.waypointParent.GetChild(i);
            }
            lastWaypoint = waypoints[waypoints.Length - 1];
        }

        indicatorController.HideAllIndicators();
    }

    private void Update()
    {
        if (waypoints == null || waypoints.Length == 0 || hasStoppedPermanently)
            return;

        if (waypointMover.IsReversing())
        {
            if (indicatorCurrentlyShowing)
            {
                indicatorController.HideAllIndicators();
                indicatorCurrentlyShowing = false;
            }
            return;
        }

        // Check if NPC is in dialogue
        NPC npc = GetComponent<NPC>();
        if (npc != null && npc.IsInDialogue())
        {
            if (indicatorCurrentlyShowing)
            {
                indicatorController.HideAllIndicators();
                indicatorCurrentlyShowing = false;
            }
            return;
        }

        // Calculate actual movement
        bool isActuallyMoving = Vector3.Distance(transform.position, lastPosition) > movementThreshold;
        lastPosition = transform.position;

        // Only proceed if we're at the last waypoint index
        if (waypointMover.IsAtLastWaypoint())
        {
            bool isNearLastWaypoint = Vector3.Distance(transform.position, lastWaypoint.position) < waypointReachedThreshold;
            bool isWaiting = waypointMover.IsWaiting();

            // For non-looping waypoints: show permanently when reached and not moving
            if (!waypointMover.loopWaypoints && isNearLastWaypoint && !isActuallyMoving)
            {
                if (!indicatorCurrentlyShowing)
                {
                    ShowAppropriateIndicator();
                    indicatorCurrentlyShowing = true;
                    hasStoppedPermanently = true;
                }
                return;
            }

            // For looping waypoints: only show during wait time at last waypoint when not moving
            if (isNearLastWaypoint && isWaiting && !isActuallyMoving)
            {
                if (!indicatorCurrentlyShowing)
                {
                    ShowAppropriateIndicator();
                    indicatorCurrentlyShowing = true;
                }
            }
            else if (indicatorCurrentlyShowing && (isActuallyMoving || !isWaiting))
            {
                indicatorController.HideAllIndicators();
                indicatorCurrentlyShowing = false;
            }
        }
        else if (indicatorCurrentlyShowing)
        {
            // Hide if we're not at the last waypoint
            indicatorController.HideAllIndicators();
            indicatorCurrentlyShowing = false;
        }
    }

    private void ShowAppropriateIndicator()
    {
        NPC npc = GetComponent<NPC>();
        if (npc != null && npc.IsFirstDialogueCompleted())
        {
            indicatorController.ShowCompletedQuestIndicator();
        }
        else
        {
            indicatorController.ShowInitialQuestIndicator();
        }
    }
}