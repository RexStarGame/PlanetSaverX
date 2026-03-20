using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Bomb : MonoBehaviour
{
    [Header("Udseende & Status")]
    public Sprite activeBombSprite;
    public SpriteRenderer spriteRenderer;

    private bool isArmed = false;
    private bool isDefused = false;
    private bool hasExploded = false;

    [Header("Timer UI")]
    public float maxCountdownTime = 30f;
    private float countdownTime;

    public Image timerFillImage;
    public TMP_Text timerText;

    [Header("Health UI")]
    public float maxHealth = 100f;
    private float currentHealth;

    // Denne image bruges først som planting bar, bagefter som health bar
    public Image healthFillImage;
    public Gradient healthColor;

    [Header("Effekter")]
    public GameObject explosionEffectPrefab;

    [Header("Lyd")]
    public AudioSource audioSource;
    public AudioClip beepSound;
    private float nextBeepTime = 0f;

    void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    void Start()
    {
        countdownTime = maxCountdownTime;
        currentHealth = maxHealth;

        // Timer UI skjules indtil bomben er armed
        if (timerFillImage != null) timerFillImage.enabled = false;
        if (timerText != null) timerText.enabled = false;

        // Health image er skjult i starten
        if (healthFillImage != null) healthFillImage.enabled = false;

        UpdateTimerVisuals();
        UpdateHealthBarVisuals();
    }

    void Update()
    {
        if (!isArmed || isDefused || hasExploded) return;

        countdownTime -= Time.deltaTime;
        countdownTime = Mathf.Max(0f, countdownTime);

        UpdateTimerVisuals();

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
            Explode();
        }
    }

    public void ArmBomb()
    {
        if (isArmed) return;

        isArmed = true;

        if (spriteRenderer != null && activeBombSprite != null)
        {
            spriteRenderer.sprite = activeBombSprite;
        }

        // Countdown bliver synlig nu
        if (timerFillImage != null) timerFillImage.enabled = true;
        if (timerText != null) timerText.enabled = true;

        // Samme image som blev brugt til loading bliver nu health bar
        if (healthFillImage != null) healthFillImage.enabled = true;

        UpdateTimerVisuals();
        UpdateHealthBarVisuals();

        Debug.Log("Bomb armed: countdown og health bar burde nu være synlige.");
    }

    private void UpdateTimerVisuals()
    {
        if (timerFillImage != null)
        {
            timerFillImage.fillAmount = maxCountdownTime > 0f
                ? countdownTime / maxCountdownTime
                : 0f;
        }

        if (timerText != null)
        {
            timerText.text = Mathf.CeilToInt(countdownTime).ToString();
        }
    }

    private void UpdateHealthBarVisuals()
    {
        if (healthFillImage != null)
        {
            float healthPercentage = maxHealth > 0f ? currentHealth / maxHealth : 0f;
            healthFillImage.fillAmount = healthPercentage;
            healthFillImage.color = healthColor.Evaluate(healthPercentage);
        }
    }

    private void PlayBeep()
    {
        if (audioSource != null && beepSound != null)
        {
            audioSource.PlayOneShot(beepSound);
        }
    }

    public void TakeDamage(float damage)
    {
        if (!isArmed || isDefused || hasExploded) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0f, currentHealth);

        UpdateHealthBarVisuals();

        if (currentHealth <= 0f)
        {
            DefuseBomb();
        }
    }

    private void DefuseBomb()
    {
        if (isDefused || hasExploded) return;

        isDefused = true;

        if (timerText != null)
        {
            timerText.enabled = true;
            timerText.color = Color.green;
            timerText.text = "DEFUSED";
        }

        if (timerFillImage != null) timerFillImage.enabled = false;
        if (healthFillImage != null) healthFillImage.enabled = false;

        Destroy(gameObject, 2f);
    }

    private void Explode()
    {
        if (hasExploded) return;

        hasExploded = true;

        if (timerText != null)
        {
            timerText.enabled = true;
            timerText.color = Color.red;
            timerText.text = "BOOM!";
        }

        if (timerFillImage != null) timerFillImage.enabled = false;
        if (healthFillImage != null) healthFillImage.enabled = false;

        if (explosionEffectPrefab != null)
        {
            GameObject explosion = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 3f);
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        Destroy(gameObject, 0.5f);
    }
}