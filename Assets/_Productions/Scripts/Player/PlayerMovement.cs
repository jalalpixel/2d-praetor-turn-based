using UnityEngine;
using Lean.Pool;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private GameObject moveIndicatorPrefab;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Camera mainCamera;

    private Rigidbody2D rb;
    private Vector2 movement;        // WASD input
    private Vector2 clickTarget;     // Mouse click target
    private bool moveByClick = false;
    private bool facingRight = true; // Base sprite faces right

    private GameObject activeIndicator; // currently spawned move indicator

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Update()
    {
        // --- WASD movement ---
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (movement.sqrMagnitude > 0.01f)
        {
            // If using keyboard input, disable click movement + clear indicator
            moveByClick = false;
            DespawnIndicator();
        }

        // --- Mouse Click movement ---
        if (Input.GetMouseButtonDown(0)) // left click
        {
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0f;
            clickTarget = new Vector2(worldPos.x, worldPos.y);
            moveByClick = true;

            // Spawn or move indicator
            SpawnMovementIndicator(worldPos);
        }

        // Normalize diagonal movement (WASD only)
        if (movement.magnitude > 1)
            movement = movement.normalized;

        // Update animator speed
        float currentSpeed = movement.sqrMagnitude > 0.01f || moveByClick ? moveSpeed : 0;
        animator.SetFloat("MoveSpeed", currentSpeed);

        // Handle flipping (keyboard only)
        if (movement.x > 0 && !facingRight)
            Flip();
        else if (movement.x < 0 && facingRight)
            Flip();
    }

    void FixedUpdate()
    {
        if (moveByClick)
        {
            // Move towards click target
            Vector2 newPos = Vector2.MoveTowards(rb.position, clickTarget, moveSpeed * Time.fixedDeltaTime);
            Vector2 dir = (clickTarget - rb.position).normalized;

            rb.MovePosition(newPos);

            // Stop if close enough
            if (Vector2.Distance(rb.position, clickTarget) < 0.05f)
            {
                moveByClick = false;
                DespawnIndicator();
            }

            // Handle flipping by target direction
            if (dir.x > 0 && !facingRight)
                Flip();
            else if (dir.x < 0 && facingRight)
                Flip();
        }
        else
        {
            // WASD movement
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void SpawnMovementIndicator(Vector3 pos)
    {
        // Despawn old one if exists
        DespawnIndicator();

        activeIndicator = LeanPool.Spawn(moveIndicatorPrefab, pos, Quaternion.identity);
    }

    private void DespawnIndicator()
    {
        if (activeIndicator != null)
        {
            LeanPool.Despawn(activeIndicator);
            activeIndicator = null;
        }
    }
}
