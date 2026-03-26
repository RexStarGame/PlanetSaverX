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

    void Start()
    {
        startPosition = transform.position;

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
            Debug.LogError("No child named 'Hitbox' found. Please create a child GameObject called 'Hitbox' with a 2D trigger collider.");
        }

        PickNewRoamTarget();
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Only move toward player if outside stopping distance
        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer > stoppingDistance)
            {
                Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, transform.position.z);
                MoveTowards(targetPosition);
            }
        }
        else
        {
            // Roaming horizontally
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
    }

    private void MoveTowards(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private void PickNewRoamTarget()
    {
        // Only pick a horizontal offset
        float randomX = Random.Range(-roamRadius, roamRadius);
        roamTarget = new Vector3(startPosition.x + randomX, transform.position.y, transform.position.z);
        lastRoamTime = Time.time;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (hitbox != null && other == player.GetComponent<Collider2D>() && Time.time - lastAttackTime >= attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
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
}