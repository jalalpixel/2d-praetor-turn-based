using UnityEngine;
using System.Collections;
using System.Linq;

public class AutoBattlerTroops : MonoBehaviour
{
    [Header("Runtime State")]
    public float currentHealth;
    public bool isDead = false;

    [Header("Combat Settings")]
    public float detectRadius = 3f; // How far it can detect opponents
    public LayerMask opponentLayer; // Layer for enemies
    public float moveSpeed = 2f;

    [SerializeField] private Transform currentTarget;
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private Animator anim;

    [Header("Stats Variable")]
    // Local copies of stats for easy reference
    public string characterName;
    public int armor;
    public float agility;
    public int damage;
    public float attackSpeed;
    public float attackRange;

    private void Start()
    {
        StartCoroutine(AutoBehaviorRoutine());
    }

    public void SetupTroopsData(CharacterData characterData)
    {
        anim.runtimeAnimatorController = characterData.characterAnimator;
        currentHealth = characterData.maxHealth;
        damage = characterData.damage;
        attackSpeed = characterData.attackSpeed;
        attackRange = characterData.attackRange;

        isDead = false;
    }

    private IEnumerator AutoBehaviorRoutine()
    {
        while (!isDead)
        {
            FindNearestTarget();

            if (currentTarget != null)
            {
                float distance = Vector2.Distance(transform.position, currentTarget.position);

                // Move towards target if not in attack range
                if (distance > attackRange)
                {
                    MoveTowardsTarget();
                }
                else
                {
                    // Stop moving and attack
                    if (!isAttacking)
                        StartCoroutine(AttackRoutine());
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void FindNearestTarget()
    {
        // Find all opponents within detect radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectRadius, opponentLayer);

        if (hits.Length > 0)
        {
            currentTarget = hits
                .OrderBy(h => Vector2.Distance(transform.position, h.transform.position))
                .First()
                .transform;
        }
        else
        {
            currentTarget = null;
        }
    }

    private void MoveTowardsTarget()
    {
        if (currentTarget == null || isAttacking || isDead) return;

        Vector2 direction = (currentTarget.position - transform.position).normalized;
        Vector2 velocity = direction * moveSpeed * 3;

        transform.position += (Vector3)velocity * Time.deltaTime;

        // Flip sprite facing direction
        if (direction.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);

        if (anim && isAttacking == false)
        {
            // Update "MoveSpeed" parameter for Animator blend tree or walk/run animations
            anim.SetFloat("MoveSpeed", velocity.magnitude);
        }
    }


    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        int attackType = Random.Range(1, 3);
        anim.Play("Attack" + attackType);

        yield return new WaitForSeconds(attackSpeed);

        if (isDead)
        {
            yield break;
        }


        if (currentTarget != null)
        {
            AutoBattlerTroops enemy = currentTarget.GetComponent<AutoBattlerTroops>();
            if (enemy != null && !enemy.isDead)
            {
                enemy.TakeDamage(damage);
            }
        }

        isAttacking = false;
        anim.SetTrigger("Idle");
    }

    public void TakeDamage(float damage)
    {
        // Apply flat armor reduction
        float finalDamage = Mathf.Max(0, damage - armor);

        // Agility dodge chance
        if (Random.value < agility)
        {
            Debug.Log($"{characterName} dodged!");
            return;
        }

        currentHealth -= finalDamage;
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        currentHealth = 0;
        if (anim) anim.SetBool("Dead", isDead);
        StopAllCoroutines();
        Debug.Log($"{characterName} died!");

        // Optional: disable collider or object
        GetComponent<Collider2D>().enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
