using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Bomb : MonoBehaviour
{
    [Header("Udseende & Status")]
    public Sprite activeBombSprite;
    public SpriteRenderer spriteRenderer;
    private bool isArmed = false;

    [Header("Timer UI (Billede & Tekst)")]
    public float maxCountdownTime = 30f;
    private float countdownTime;
    public Image timerFillImage;
    public TMP_Text timerText;

    [Header("Liv og Defuse (Kun Billede)")]
    public float maxHealth = 100f;
    private float currentHealth;
    public Image healthFillImage;
    public Gradient healthColor;

    [Header("Effekter")]
    public GameObject explosionEffectPrefab;

    [Header("Lyd")]
    public AudioSource audioSource;
    public AudioClip beepSound;
    private float nextBeepTime = 0f;

    private bool isDefused = false;
    private bool hasExploded = false;

    void Awake()
    {
        // [NYT] Sikrer, at vi altid har fat i SpriteRenderer, 
        // selv hvis du har glemt at trække den ind i Inspectoren!
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    void Start()
    {
        countdownTime = maxCountdownTime;
        currentHealth = maxHealth;

        // Skjul bombens nedtællings-UI og tekst, mens den bliver plantet
        if (timerFillImage != null) timerFillImage.gameObject.SetActive(false);
        if (timerText != null) timerText.gameObject.SetActive(false);
        if (healthFillImage != null) healthFillImage.gameObject.SetActive(false);
    }

    void Update()
    {
        // Hvis bomben ikke er færdigplantet endnu, gør vi INGENTING
        if (!isArmed || isDefused || hasExploded) return;

        countdownTime -= Time.deltaTime;

        // Opdater billede-baren
        if (timerFillImage != null)
        {
            timerFillImage.fillAmount = countdownTime / maxCountdownTime;
        }

        // Opdater tallene på skærmen
        if (timerText != null)
        {
            timerText.text = Mathf.CeilToInt(countdownTime).ToString();
        }

        // Bip-logik
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
    }

    // Denne funktion kaldes af PlayerBombPlanter, NØJAGTIGT når tiden (3.0f) rammes
    public void ArmBomb()
    {
        isArmed = true;

        // [VIGTIGT] Tvinger skiftet til din aktive pixel art sprite!
        if (spriteRenderer != null && activeBombSprite != null)
        {
            spriteRenderer.sprite = activeBombSprite;
            Debug.Log("Bomben er armeret! Billedet burde være skiftet nu."); // Lille tjek i din Console
        }
        else
        {
            Debug.LogWarning("Manglende reference! Har du husket at trække 'Active Bomb Sprite' ind i Inspectoren?");
        }

        // Vis bombens egen UI (timer, defuse bar og tekst)
        if (timerFillImage != null) timerFillImage.gameObject.SetActive(true);
        if (timerText != null) timerText.gameObject.SetActive(true);
        if (healthFillImage != null) healthFillImage.gameObject.SetActive(true);

        UpdateHealthBarVisuals();
    }

    void UpdateHealthBarVisuals()
    {
        if (healthFillImage != null)
        {
            float healthPercentage = currentHealth / maxHealth;
            healthFillImage.fillAmount = healthPercentage;
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
        if (!isArmed || isDefused || hasExploded) return;

        currentHealth -= damage;
        UpdateHealthBarVisuals();

        if (currentHealth <= 0)
        {
            currentHealth = 0;
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
        if (healthFillImage != null) healthFillImage.gameObject.SetActive(false);
        if (timerFillImage != null) timerFillImage.gameObject.SetActive(false);

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
        if (healthFillImage != null) healthFillImage.gameObject.SetActive(false);
        if (timerFillImage != null) timerFillImage.gameObject.SetActive(false);

        if (explosionEffectPrefab != null)
        {
            GameObject explosion = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 3f);
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false; // Gem selve bomben, når den sprænger
        }

        Destroy(gameObject, 0.5f);
    }
}