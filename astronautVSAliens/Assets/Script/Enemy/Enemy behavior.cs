using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [Header("Targets")]
    public Transform player;
    public Transform bomb;

    [Header("Detection Settings")]
    public float detectionRange = 10f;
    public float stoppingDistance = 1.5f;

    [Header("Distance Settings")]
    public float bombStoppingDistance = 2.5f;

    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float bombMoveSpeed = 1.5f;
    public float roamRadius = 5f;
    public float roamDelay = 2f;

    [Header("Combat Settings")]
    public int damage = 10;
    public float attackCooldown = 1f;

    [Header("Bomb Settings")]
    public string bombTag = "Bomb";

    private Vector3 startPosition;
    private Vector3 roamTarget;
    private float lastRoamTime;
    private float lastAttackTime;

    private Collider2D hitbox;
    private HealthBar playerHealth;

    private Rigidbody2D rb;
    private bool playerInHitbox = false;
    private bool playerOnTop = false;

    private bool targetingBomb = false;
    private float bombCheckTimer = 0f;
    private float bombCheckInterval = 0.5f;

    void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();

        if (player != null)
        {
            playerHealth = player.GetComponent<HealthBar>();
        }

        Transform hitboxTransform = transform.Find("Hitbox");
        if (hitboxTransform != null)
        {
            hitbox = hitboxTransform.GetComponent<Collider2D>();
        }

        PickNewRoamTarget();

        // Try to find bomb if it already exists in scene
        FindBomb();
    }

    void Update()
    {
        if (player == null) return;

        // Continuously look for bomb if we don't have one or if it's inactive
        if (bomb == null || !bomb.gameObject.activeInHierarchy)
        {
            bombCheckTimer += Time.deltaTime;
            if (bombCheckTimer >= bombCheckInterval)
            {
                bombCheckTimer = 0f;
                FindBomb();
            }
        }

        // Stop if player is on top
        if (playerOnTop)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // Target selection
        targetingBomb = false;

        if (bomb != null && bomb.gameObject.activeInHierarchy)
        {
            float distToPlayer = Vector3.Distance(transform.position, player.position);
            float distToBomb = Vector3.Distance(transform.position, bomb.position);

            if (distToBomb < distToPlayer)
            {
                targetingBomb = true;
            }
        }

        // Set current target and speed
        Transform currentTarget = targetingBomb ? bomb : player;
        float currentSpeed = targetingBomb ? bombMoveSpeed : moveSpeed;
        float currentStoppingDistance = targetingBomb ? bombStoppingDistance : stoppingDistance;

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

        // Chase or roam
        if (distanceToTarget <= detectionRange)
        {
            if (distanceToTarget > currentStoppingDistance)
            {
                Vector3 targetPosition = new Vector3(currentTarget.position.x, transform.position.y, transform.position.z);
                MoveTowards(targetPosition, currentSpeed);
            }
            else
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
        }
        else
        {
            float distanceToRoamTarget = Vector3.Distance(transform.position, roamTarget);

            if (distanceToRoamTarget < 0.2f || Time.time - lastRoamTime >= roamDelay)
            {
                PickNewRoamTarget();
            }
            else
            {
                Vector3 targetPosition = new Vector3(roamTarget.x, transform.position.y, transform.position.z);
                MoveTowards(targetPosition, moveSpeed);
            }
        }

        // Attack player only when not targeting bomb
        if (!targetingBomb && playerInHitbox && Time.time - lastAttackTime >= attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    private void FindBomb()
    {
        GameObject bombObject = GameObject.FindGameObjectWithTag(bombTag);
        if (bombObject != null)
        {
            bomb = bombObject.transform;
            Debug.Log("Enemy found bomb with tag: " + bombTag);
        }
    }

    private void MoveTowards(Vector3 target, float speed)
    {
        float directionX = Mathf.Sign(target.x - transform.position.x);
        rb.linearVelocity = new Vector2(directionX * speed, rb.linearVelocity.y);
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