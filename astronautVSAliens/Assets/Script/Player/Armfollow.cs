using UnityEngine;

public class ArmAim : MonoBehaviour
{
    public Transform player;          // Reference to the player
    public bool facingRight = true;   // Keep in sync with your player script

    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePos - player.position;
        direction.z = 0f;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (facingRight)
        {
            // Normal rotation
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        else
        {
            // Flip for left side
            transform.rotation = Quaternion.Euler(180f, 0f, -angle);
        }
    }
}