using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 2f;

    private Rigidbody2D rb;

    private bool isDestroyed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.right * speed;
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (isDestroyed) return;

        if (hitInfo.CompareTag("Player")) return;
        isDestroyed = true;
        Debug.Log("Ramte: " + hitInfo.name);

        Destroy(gameObject);
    }
}