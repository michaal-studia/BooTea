using UnityEngine;

public class PointerController : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public RectTransform safeZone;
    public static float minSpeed = 250f;
    public static float maxSpeed = 650f;
    public static float speedIncreaseFactor = 1.15f;
    public float moveSpeed = minSpeed;
    public CookingPanel cookingPanel;

    private float direction = 1f;
    private RectTransform pointerTransform;
    private Vector3 targetPosition;

    void Start()
    {
        pointerTransform = GetComponent<RectTransform>();
        if (pointerTransform == null)
        {
            Debug.LogError("PointerTransform nie został znaleziony!");
            enabled = false; // Wyłącza skrypt, aby zapobiec błędom
            return;
        }

        if (pointA == null || pointB == null || safeZone == null)
        {
            Debug.LogError("Brak przypisanych PointA, PointB lub SafeZone!");
            enabled = false;
            return;
        }

        targetPosition = pointB.position;
    }

    void Update()
    {
        if (pointerTransform == null) return;

        pointerTransform.position = Vector3.MoveTowards(pointerTransform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(pointerTransform.position, pointA.position) < 0.1f)
        {
            targetPosition = pointB.position;
            direction = 1f;
            if (moveSpeed < maxSpeed)
                moveSpeed *= speedIncreaseFactor;
        }
        else if (Vector3.Distance(pointerTransform.position, pointB.position) < 0.1f)
        {
            targetPosition = pointA.position;
            direction = -1f;
            if (moveSpeed < maxSpeed)
                moveSpeed *= speedIncreaseFactor;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckSuccess();
        }
    }

    public void ResetPointer()
    {
        if (pointerTransform == null) return; // Zabezpieczenie

        pointerTransform.position = pointA.position;
        targetPosition = pointB.position;
        moveSpeed = minSpeed; // Reset prędkości
    }

    void CheckSuccess()
    {
        if (safeZone == null || pointerTransform == null) return;

        bool success = RectTransformUtility.RectangleContainsScreenPoint(safeZone, pointerTransform.position, null);
        cookingPanel.FinishQTE(success);
    }
}