using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Gun : MonoBehaviour
{
    [Header("Skyde Indstillinger")]
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public float fireRate = 0.2f;

    [Header("Ammo & Reload Indstillinger")]
    public int maxAmmo = 10;
    public float reloadTime = 1.5f;
    public float autoReloadDelay = 1f;

    [Header("UI Indstillinger")]
    public Image ammoBar;

    private int currentAmmo;
    private float nextFireTime;
    private float lastShotTime;
    private bool isReloading = false;

    void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
    }

    void Update()
    {
        if (isReloading) return;

        // Auto-reload tjek
        if (currentAmmo < maxAmmo && Time.time >= lastShotTime + autoReloadDelay && !Input.GetMouseButton(0))
        {
            StartCoroutine(Reload());
            return;
        }

        // Skyde-logik
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            if (currentAmmo > 0)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
            else
            {
                StartCoroutine(Reload());
            }
        }
    }

    void Shoot()
    {
        currentAmmo--;
        lastShotTime = Time.time;
        UpdateAmmoUI();

        if (bulletPrefab != null && shootPoint != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;

            Vector3 shootDirection = mousePos - shootPoint.position;
            float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;

            Quaternion correctRotation = Quaternion.Euler(0f, 0f, angle);
            Instantiate(bulletPrefab, shootPoint.position, correctRotation);
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;

        // Gør UI-bjælken lidt gennemsigtig
        if (ammoBar != null)
        {
            Color tempColor = ammoBar.color;
            tempColor.a = 0.5f;
            ammoBar.color = tempColor;
        }

        // --- SMOOTH RELOAD LOGIK ---
        float reloadTimer = 0f;
        // Gemmer hvor bjælken var, da vi startede med at reloade
        float startFillAmount = ammoBar != null ? ammoBar.fillAmount : 0f;

        // Kør denne løkke indtil timer-en når vores fastsatte reloadTime
        while (reloadTimer < reloadTime)
        {
            reloadTimer += Time.deltaTime; // Læg tiden for én frame til

            if (ammoBar != null)
            {
                // Lerp udregner en blød overgang fra startFillAmount til 1 (100%) baseret på tiden
                ammoBar.fillAmount = Mathf.Lerp(startFillAmount, 1f, reloadTimer / reloadTime);
            }

            yield return null; // VIGTIGT: Vent til næste frame, før vi opdaterer igen
        }

        // Sørg for, at værdierne er helt præcise, når løkken slutter
        currentAmmo = maxAmmo;
        UpdateAmmoUI(); // Sætter bjælken til præcis 100%

        // Gør UI-bjælken fuldt synlig igen
        if (ammoBar != null)
        {
            Color tempColor = ammoBar.color;
            tempColor.a = 1f;
            ammoBar.color = tempColor;
        }

        isReloading = false;
    }

    void UpdateAmmoUI()
    {
        if (ammoBar != null)
        {
            ammoBar.fillAmount = (float)currentAmmo / maxAmmo;
        }
    }
}