using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI")]
    public Image healthBarImage;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateHealthBar();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        float fillValue = currentHealth / maxHealth;
        healthBarImage.fillAmount = fillValue;
    }

    void Die()
    {
        Debug.Log("Player died!");
        // Add death logic here (disable movement, play animation, etc.)
    }
}