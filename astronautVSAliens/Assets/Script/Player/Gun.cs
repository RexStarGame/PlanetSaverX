using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bulletPrefab; // Træk din Bullet Prefab herind
    public Transform shootPoint;    // Lav et tomt objekt forrest på løbet og træk det herind
    public float fireRate = 0.2f;   // Hvor hurtigt man kan skyde

    private float nextFireTime;

    void Update()
    {
        // Skyder når man trykker på venstre musetast (0)
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if (bulletPrefab != null && shootPoint != null)
        {
            // 1. Find musens position i verden
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;

            // 2. Tegn en usynlig linje direkte fra shootPoint til musen
            Vector3 shootDirection = mousePos - shootPoint.position;

            // 3. Omregn den linje til en vinkel i grader
            float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;

            // 4. Skab en helt ren rotation baseret på vinklen (ignorerer spillerens flip)
            Quaternion correctRotation = Quaternion.Euler(0f, 0f, angle);

            // 5. Skab kuglen
            Instantiate(bulletPrefab, shootPoint.position, correctRotation);
        }
    }
}