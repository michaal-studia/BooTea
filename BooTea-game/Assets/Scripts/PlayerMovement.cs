using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    private Vector2 movementDirection;
    private Vector2 lastValidDirection;
    private bool playingFootsteps = false;
    public float footstepSpeed = 0.5f;

    [SerializeField] private float sprintMultiplier = 1.75f;
    [SerializeField] private float sprintFootstepSpeed = 1.5f;

    private bool isSprinting = false;

    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (PauseController.IsGamePaused)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isWalking", false);
            animator.speed = 1f;
            StopFootsteps();
            return;
        }

        float currentSpeed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;
        float currentFootstepSpeed = isSprinting ? sprintFootstepSpeed : footstepSpeed;

        rb.linearVelocity = movementDirection * currentSpeed;

        animator.SetBool("isWalking", rb.linearVelocity.magnitude > 0);
        animator.speed = isSprinting ? 1.5f : 1f;

        if (rb.linearVelocity.magnitude > 0 && !playingFootsteps)
        {
            StartFootsteps();
        }
        else if (rb.linearVelocity.magnitude == 0)
        {
            StopFootsteps();
        }
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
        animator.SetBool("isWalking", movementDirection != Vector2.zero);
        animator.SetFloat("InputX", movementDirection.x);
        animator.SetFloat("InputY", movementDirection.y);
    }

    public void SetSprinting(bool value)
    {
        isSprinting = value;
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        if (context.performed)
            isSprinting = true;
        else if (context.canceled)
            isSprinting = false;

        if (rb.linearVelocity.magnitude > 0)
        {
            StopFootsteps();
            StartFootsteps();
        }
    }

    void StartFootsteps()
    {
        //playingFootsteps = true;
        //float delay = isSprinting ? sprintFootstepSpeed : footstepSpeed;
        //InvokeRepeating(nameof(PlayFootsteps), 0f, delay);
    }

    void StopFootsteps()
    {
        playingFootsteps = false;
        CancelInvoke(nameof(PlayFootsteps));
    }

    public void PlayFootsteps()
    {
        if (audioManager == null || audioManager.footsteps == null) return;

        float randomPitch = Random.Range(0.95f, 1.15f);
        audioManager.PlayFootstepSFX(randomPitch);
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