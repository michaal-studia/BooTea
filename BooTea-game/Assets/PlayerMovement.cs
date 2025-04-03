using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    private Vector2 movementDirection;
    private Vector2 lastValidDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (PauseController.IsGamePaused)
        {
            rb.linearVelocity = Vector2.zero; //stop movement
            animator.SetBool("isWalking", false);
            return;
        }
        rb.linearVelocity = movementDirection * moveSpeed;
        animator.SetBool("isWalking", rb.linearVelocity.magnitude > 0);
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputX", lastValidDirection.x);
            animator.SetFloat("LastInputY", lastValidDirection.y);
            movementDirection = Vector2.zero;
            return;
        }
        
        moveInput = context.ReadValue<Vector2>();
        Vector2 newDirection = GetDirection(moveInput);
        
        if (newDirection != Vector2.zero)
        {
            lastValidDirection = newDirection;
        }
        
        movementDirection = newDirection;
        animator.SetFloat("InputX", movementDirection.x);
        animator.SetFloat("InputY", movementDirection.y);
    }

    private Vector2 GetDirection(Vector2 input)
    {
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            return new Vector2(Mathf.Sign(input.x), 0); // Priorytet poziomy
        }
        else if (Mathf.Abs(input.y) > Mathf.Abs(input.x))
        {
            return new Vector2(0, Mathf.Sign(input.y)); // Priorytet pionowy
        }
        return lastValidDirection; // Nie resetuj do zera, jeśli naciskane są dwa przyciski
    }
}