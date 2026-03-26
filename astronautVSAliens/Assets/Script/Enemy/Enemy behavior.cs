using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform player;
    public float detectionRange = 10f;
    public float stoppingDistance = 1.5f;

    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float roamRadius = 5f;
    public float roamDelay = 2f;

    [Header("Combat Settings")]
    public int damage = 10;
    public float attackCooldown = 1f;

    private Vector3 startPosition;
    private Vector3 roamTarget;
    private float lastRoamTime;
    private float lastAttackTime;

    private Collider2D hitbox;
    private HealthBar playerHealth;

    private Rigidbody2D rb;
    private bool playerInHitbox = false;
    private bool playerOnTop = false; // NEW

    void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();

        if (player != null)
        {
            playerHealth = player.GetComponent<HealthBar>();
            if (playerHealth == null)
                Debug.LogError("Player does not have a HealthBar component!");
        }

        Transform hitboxTransform = transform.Find("Hitbox");
        if (hitboxTransform != null)
        {
            hitbox = hitboxTransform.GetComponent<Collider2D>();
            if (hitbox == null)
                Debug.LogError("Hitbox object found but has no 2D Collider component!");
        }
        else
        {
            Debug.LogError("No child named 'Hitbox' found. Please create one with a 2D trigger collider.");
        }

        PickNewRoamTarget();
    }

    void Update()
    {
        if (player == null) return;

        // STOP movement if player is on top
        if (playerOnTop)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float heightDifference = Mathf.Abs(player.position.y - transform.position.y);

        // Chase player
        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer > stoppingDistance && heightDifference < 1.0f)
            {
                Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, transform.position.z);
                MoveTowards(targetPosition);
            }
        }
        else
        {
            // Roaming
            float distanceToRoamTarget = Vector3.Distance(transform.position, roamTarget);

            if (distanceToRoamTarget < 0.2f || Time.time - lastRoamTime >= roamDelay)
            {
                PickNewRoamTarget();
            }
            else
            {
                Vector3 targetPosition = new Vector3(roamTarget.x, transform.position.y, transform.position.z);
                MoveTowards(targetPosition);
            }
        }

        // Attack logic
        if (playerInHitbox && Time.time - lastAttackTime >= attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    private void MoveTowards(Vector3 target)
    {
        Vector2 direction = (target - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
    }

    private void PickNewRoamTarget()
    {
        float randomX = Random.Range(-roamRadius, roamRadius);
        roamTarget = new Vector3(startPosition.x + randomX, transform.position.y, transform.position.z);
        lastRoamTime = Time.time;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInHitbox = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInHitbox = false;
        }
    }

    private void Attack()
    {
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            Debug.Log("Enemy attacked player for " + damage + " damage!");
        }
    }

    // --- NEW: Detect player standing on top ---
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y < -0.5f)
                {
                    playerOnTop = true;
                    return;
                }
            }
        }
        playerOnTop = false;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnTop = false;
        }
    }
}