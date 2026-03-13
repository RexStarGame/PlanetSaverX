using UnityEngine;
using UnityEngine.UI;

public class PlantZone : MonoBehaviour
{
    [Header("Bombe Indstillinger")]
    public GameObject bombPrefab;
    public float timeToPlant = 3.0f;

    [Header("UI Indikator")]
    public Slider progressBar;

    // Denne variabel er låsen, der sikrer, at man KUN kan plante her
    private bool isPlayerInZone = false;
    private float currentPlantTime = 0f;

    void Start()
    {
        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(false);
            progressBar.value = 0f;
        }
    }

    void Update()
    {
        if (isPlayerInZone)
        {
            if (Input.GetKey(KeyCode.E))
            {
                currentPlantTime += Time.deltaTime;
                UpdateUI();

                if (currentPlantTime >= timeToPlant)
                {
                    PlantBomb();
                }
            }
            else
            {
                ResetPlanting();
            }
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            ResetPlanting();
        }
    }

    void PlantBomb()
    {
        Instantiate(bombPrefab, transform.position, Quaternion.identity);

        ResetPlanting();
        gameObject.SetActive(false);
    }

    void UpdateUI()
    {
        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(true);
            progressBar.value = currentPlantTime / timeToPlant;
        }
    }

    void ResetPlanting()
    {
        currentPlantTime = 0f;

        if (progressBar != null)
        {
            progressBar.value = 0f;
            progressBar.gameObject.SetActive(false);
        }
    }
}