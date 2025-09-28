using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TroopsMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stopRadius = 1.5f; // how close troop can get to player
    private bool facingRight = true;

    [Header("References")]
    [SerializeField] private Transform player;

    private Rigidbody2D rb;
    [SerializeField] private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // Direction to player
        Vector2 dir = (player.position - transform.position).normalized;
        float dist = Vector2.Distance(transform.position, player.position);

        if (dist > stopRadius)
        {
            // Move towards player if outside radius
            rb.linearVelocity = dir * moveSpeed;
        }
        else
        {
            // Stop moving if within radius
            rb.linearVelocity = Vector2.zero;
        }

        // Handle flipping by target direction
        if (dir.x > 0 && !facingRight)
            Flip();
        else if (dir.x < 0 && facingRight)
            Flip();

        // Set animation speed (magnitude of velocity)
        float currentSpeed = rb.linearVelocity.magnitude;
        anim.SetFloat("MoveSpeed", currentSpeed);
    }

    private void Flip()
    {
        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

}
