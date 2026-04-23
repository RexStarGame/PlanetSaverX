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
    public int playerDamage = 10;
    public int bombDamage = 25;
    public float attackCooldown = 1f;
    public float bombAttackCooldown = 0.5f;

    [Header("Bomb Settings")]
    public string bombTag = "Bomb";

    private Vector3 startPosition;
    private Vector3 roamTarget;
    private float lastRoamTime;
    private float lastAttackTime;
    private float lastBombAttackTime;

    private Collider2D hitbox;
    private HealthBar playerHealth;
    private Bomb bombScript;

    private Rigidbody2D rb;
    private bool playerInHitbox = false;
    private bool bombInHitbox = false;
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
            if (hitbox != null)
            {
                Debug.Log("Hitbox found and assigned");
            }
            else
            {
                Debug.LogError("Hitbox object has no Collider2D component!");
            }
        }
        else
        {
            Debug.LogError("No child named 'Hitbox' found. Make sure your enemy has a child object called 'Hitbox' with a Collider2D set to IsTrigger=true");
        }

        PickNewRoamTarget();

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

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

        // Chase or roam
        if (distanceToTarget <= detectionRange)
        {
            if (distanceToTarget > stoppingDistance)
            {
                Vector3 targetPosition = new Vector3(currentTarget.position.x, transform.position.y, transform.position.z);
                MoveTowards(targetPosition, currentSpeed);
            }
            else
            {
                // Stop moving when close enough
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

        // Attack logic with debug
        if (targetingBomb)
        {
            if (bombInHitbox)
            {
                if (Time.time - lastBombAttackTime >= bombAttackCooldown)
                {
                    AttackBomb();
                    lastBombAttackTime = Time.time;
                }
            }
            else
            {
                // Debug to see if we're targeting bomb but not in hitbox
                if (Time.frameCount % 120 == 0)
                {
                    float distToBomb = Vector3.Distance(transform.position, bomb.position);
                    Debug.Log($"Targeting bomb but not in hitbox. Distance to bomb: {distToBomb:F2}");
                }
            }
        }
        else
        {
            if (playerInHitbox && Time.time - lastAttackTime >= attackCooldown)
            {
                AttackPlayer();
                lastAttackTime = Time.time;
            }
        }
    }

    private void FindBomb()
    {
        GameObject bombObject = GameObject.FindGameObjectWithTag(bombTag);
        if (bombObject != null)
        {
            bomb = bombObject.transform;
            bombScript = bombObject.GetComponent<Bomb>();
            Debug.Log("Enemy found bomb with tag: " + bombTag);

            // Check if bomb has a collider
            Collider2D bombCollider = bombObject.GetComponent<Collider2D>();
            if (bombCollider != null)
            {
                Debug.Log($"Bomb collider found: {bombCollider.GetType().Name}, IsTrigger: {bombCollider.isTrigger}");
            }
            else
            {
                Debug.LogError("Bomb has no Collider2D component! Add a Collider2D to the bomb.");
            }
        }
        else
        {
            // Try to find bomb by name if tag doesn't work
            bombObject = GameObject.Find("Bomb");
            if (bombObject != null)
            {
                bomb = bombObject.transform;
                bombScript = bombObject.GetComponent<Bomb>();
                Debug.LogWarning("Enemy found bomb by name, but it doesn't have the 'Bomb' tag. Consider adding the tag.");
            }
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
        Debug.Log($"Trigger entered with: {other.gameObject.name}, Tag: {other.tag}");

        if (other.CompareTag("Player"))
        {
            playerInHitbox = true;
            Debug.Log("Player entered hitbox");
        }
        else if (other.CompareTag("Bomb"))
        {
            bombInHitbox = true;
            Debug.Log("Bomb entered hitbox");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"Trigger exited with: {other.gameObject.name}, Tag: {other.tag}");

        if (other.CompareTag("Player"))
        {
            playerInHitbox = false;
            Debug.Log("Player exited hitbox");
        }
        else if (other.CompareTag("Bomb"))
        {
            bombInHitbox = false;
            Debug.Log("Bomb exited hitbox");
        }
    }

    private void AttackPlayer()
    {
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(playerDamage);
            Debug.Log("Enemy attacked player for " + playerDamage + " damage!");
        }
    }

    private void AttackBomb()
    {
        if (bombScript != null)
        {
            bombScript.TakeDamage(bombDamage);
            Debug.Log("Enemy attacked bomb for " + bombDamage + " damage!");
        }
        else
        {
            Debug.LogError("Bomb script is null! Cannot damage bomb.");
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