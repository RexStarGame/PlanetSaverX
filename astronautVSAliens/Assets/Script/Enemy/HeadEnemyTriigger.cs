using UnityEngine;

public class EnemyHeadTrigger : MonoBehaviour
{
    public float pushForce = 5f; // Horizontal push strength

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                // Determine push direction: left or right
                float pushDirection = other.transform.position.x > transform.position.x ? 1f : -1f;
                playerRb.linearVelocity = new Vector2(pushDirection * pushForce, playerRb.linearVelocity.y);
            }
        }
    }
}