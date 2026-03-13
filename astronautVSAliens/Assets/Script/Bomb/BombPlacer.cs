using UnityEngine;
using UnityEngine.UI; // Nødvendigt for at bruge UI elementer som Slider

public class BombPlacer : MonoBehaviour
{
    [Header("Indstillinger")]
    public GameObject bombPrefab;       // Bomben der skal spawnes
    public float timeToPlant = 3.0f;    // Hvor lang tid E skal holdes nede (i sekunder)

    [Header("UI Indikator")]
    public Slider progressBar;          // Træk din UI Slider ind her fra Inspectoren

    private bool isInZone = false;
    private bool isPlanting = false;
    private float currentPlantTime = 0f;
    private Transform currentZone;      // Gemmer zonen vi står i, så bomben kan placeres der

    void Start()
    {
        // Sørg for at progress baren er skjult fra start
        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Vi kan kun starte plantningen, hvis vi er i en zone
        if (isInZone)
        {
            // Mens man HOLDER 'E' nede
            if (Input.GetKey(KeyCode.E))
            {
                isPlanting = true;
                currentPlantTime += Time.deltaTime; // Læg tiden der går, til vores tæller
                UpdateUI();

                // Tjek om vi har holdt knappen nede længe nok
                if (currentPlantTime >= timeToPlant)
                {
                    PlantBomb();
                }
            }
            // Hvis man SLIPPER 'E', før tiden er gået
            else if (Input.GetKeyUp(KeyCode.E))
            {
                ResetPlanting();
            }
        }
        // Hvis man går ud af zonen, mens man prøver at plante
        else if (isPlanting)
        {
            ResetPlanting();
        }
    }

    // Når spilleren går IND i en trigger
    void OnTriggerEnter(Collider other)
    {
        // Tjek om det vi går ind i, har "PlantZone" scriptet på sig
        if (other.GetComponent<PlantZone>() != null)
        {
            isInZone = true;
            currentZone = other.transform;
        }
    }

    // Når spilleren går UD af en trigger
    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlantZone>() != null)
        {
            isInZone = false;
            currentZone = null;
            ResetPlanting(); // Afbryd plantningen, hvis man forlader zonen
        }
    }

    void PlantBomb()
    {
        // Spawner bomben præcis i midten af zonen (du kan også bruge transform.position for at spawne den ved spilleren)
        Instantiate(bombPrefab, currentZone.position, Quaternion.identity);

        // Valgfrit: Deaktiver zonen, så man ikke kan plante flere bomber samme sted
        currentZone.gameObject.SetActive(false);

        ResetPlanting();
        isInZone = false;
    }

    void UpdateUI()
    {
        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(true);
            // Udregner en værdi mellem 0 og 1 til vores slider
            progressBar.value = currentPlantTime / timeToPlant;
        }
    }

    void ResetPlanting()
    {
        isPlanting = false;
        currentPlantTime = 0f;

        if (progressBar != null)
        {
            progressBar.value = 0f;
            progressBar.gameObject.SetActive(false); // Skjul slideren igen
        }
    }
}