using UnityEngine;

public class ArmAim : MonoBehaviour
{
    public Transform player; // Skal være din Parent spiller

    [Header("Aim Indstillinger")]
    public float minAngle = -90f; // Prøv at sætte disse bredt først
    public float maxAngle = 90f;

    void Update()
    {
        // 1. Sikkerhedstjek
        if (player == null || Camera.main == null) return;

        // 2. Musens position i verden
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = player.position.z; // Hold musen på samme dybde som spilleren

        // 3. Konverter til lokale koordinater (relativt til player)
        Vector3 localMouse = player.InverseTransformPoint(mouseWorld);

        // 4. Find retning og vinkel
        // Vi bruger Vector2 her for at sikre os, at vi kun kigger på X og Y
        Vector2 direction = (Vector2)localMouse - (Vector2)transform.localPosition;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 5. Begræns rotationen
        angle = Mathf.Clamp(angle, minAngle, maxAngle);

        // 6. Påfør rotation LOKALT
        transform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
}