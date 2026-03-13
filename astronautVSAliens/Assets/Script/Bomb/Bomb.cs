using UnityEngine;
using TMPro; // For TextMeshPro
using UnityEngine.UI; // [NEW] Required for the Health Slider

public class Bomb : MonoBehaviour
{
    [Header("Timer og UI")]
    public float countdownTime = 30f;
    public TMP_Text timerText;

    [Header("Liv og Defuse")]
    public float maxHealth = 100f;
    private float currentHealth;
    public Slider healthSlider;       // [NEW] Drag the UI Slider here
    public Gradient healthColor;     // [NEW] Optional: Change color from Green to Red
    public Image healthFillImage;    // [NEW] The 'Fill' image of the slider

    [Header("Effekter")]
    public GameObject explosionEffectPrefab; // [NEW] Drag your Explosion Prefab (Particle System) here

    [Header("Lyd")]
    public AudioSource audioSource;
    public AudioClip beepSound;
    private float nextBeepTime = 0f;

    private bool isDefused = false;
    private bool hasExploded = false;

    void Start()
    {
        currentHealth = maxHealth;

        // [NEW] Initialize Health Bar
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
            UpdateHealthBarColor();
        }

        UpdateTimerText();
    }

    void Update()
    {
        if (isDefused || hasExploded) return;

        countdownTime -= Time.deltaTime;

        // Beep logic (last 10 seconds)
        if (countdownTime <= 10f && countdownTime > 0f)
        {
            if (Time.time >= nextBeepTime)
            {
                PlayBeep();
                nextBeepTime = Time.time + 1f;
            }
        }

        if (countdownTime <= 0f)
        {
            countdownTime = 0f;
            Explode();
        }

        UpdateTimerText();
    }

    void UpdateTimerText()
    {
        if (timerText != null)
        {
            timerText.text = Mathf.Ceil(countdownTime).ToString();
        }
    }

    // [NEW] Updates the slider value and visual color
    void UpdateHealthBarVisuals()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
            UpdateHealthBarColor();
        }
    }

    // [NEW] Makes the bar go from Green (full) to Red (empty)
    void UpdateHealthBarColor()
    {
        if (healthFillImage != null)
        {
            // Calculate 0.0 to 1.0 value based on health
            float healthPercentage = currentHealth / maxHealth;
            // Set color based on the Gradient defined in Inspector
            healthFillImage.color = healthColor.Evaluate(healthPercentage);
        }
    }

    void PlayBeep()
    {
        if (audioSource != null && beepSound != null)
        {
            audioSource.PlayOneShot(beepSound);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDefused || hasExploded) return;

        currentHealth -= damage;

        // [NEW] Update the UI immediately
        UpdateHealthBarVisuals();

        if (currentHealth <= 0)
        {
            currentHealth = 0; // Don't go below zero
            DefuseBomb();
        }
    }

    void DefuseBomb()
    {
        isDefused = true;

        if (timerText != null)
        {
            timerText.color = Color.green;
            timerText.text = "DEFUSED";
        }

        // Hide health bar when defused
        if (healthSlider != null) healthSlider.gameObject.SetActive(false);

        // Later: Add a defused sound or animation here
        Destroy(gameObject, 2f);
    }

    void Explode()
    {
        hasExploded = true;

        if (timerText != null)
        {
            timerText.color = Color.red;
            timerText.text = "BOOM!";
        }

        // Hide health bar during explosion
        if (healthSlider != null) healthSlider.gameObject.SetActive(false);

        // [NEW] Spawn the Explosion Effect
        if (explosionEffectPrefab != null)
        {
            // Instantiate the prefab at the bomb's position
            GameObject explosion = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

            // If it's a Particle System, make sure the Prefab is set to "Stop Action: Destroy"
            // or destroy it via script after a set time:
            Destroy(explosion, 3f);
        }

        // Hide the bomb mesh so it looks like it's gone, but keep script running for a moment if needed
        GetComponent<SpriteRenderer>().enabled = false;

        Destroy(gameObject, 0.5f);
    }
}