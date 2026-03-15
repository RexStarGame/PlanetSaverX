using UnityEngine;

public class PlayerBombPlanter : MonoBehaviour
{
    [Header("Planting Settings")]
    public float timeToPlant = 3.0f;
    private float currentProgress = 0f;

    [Header("Bomb Reference")]
    public GameObject bombPrefab;
    private GameObject spawnedBomb;
    private BombVisuals bombVisualsScript;

    // Status variabler
    private bool isInPlantZone = false;
    private bool isBombFullyPlanted = false;
    private Transform plantLocation;

    void Update()
    {
        if (isBombFullyPlanted) return;

        // Tjekker om spilleren er i zonen OG holder E inde
        if (isInPlantZone && Input.GetKey(KeyCode.E))
        {
            // 1. KUN ÉN BOMBE. Spawner den allerførste gang E trykkes.
            if (spawnedBomb == null)
            {
                spawnedBomb = Instantiate(bombPrefab, plantLocation.position, Quaternion.identity);
                bombVisualsScript = spawnedBomb.GetComponent<BombVisuals>();
            }

            // 2. VIS BOMBE UI & ØG PROGRESSION
            if (bombVisualsScript != null)
            {
                bombVisualsScript.ShowPlantingUI(); // Sørg for at den vises, mens vi holder E
            }

            currentProgress += Time.deltaTime;
            float progressPercent = currentProgress / timeToPlant;

            // 3. OPDATER UI IMAGE PÅ BOMBEN
            if (bombVisualsScript != null)
            {
                bombVisualsScript.UpdateVisuals(progressPercent);
            }

            // 4. ER VI FÆRDIGE?
            if (currentProgress >= timeToPlant)
            {
                isBombFullyPlanted = true;

                if (bombVisualsScript != null)
                {
                    bombVisualsScript.HidePlantingUI(); // Skjul baren når vi er færdige
                }

                // Hent Bomb-scriptet og armér bomben
                Bomb activeBombLogic = spawnedBomb.GetComponent<Bomb>();
                if (activeBombLogic != null)
                {
                    activeBombLogic.ArmBomb();
                }
            }
        }
        else
        {
            // Hvis spilleren slipper 'E' (og bomben findes), skjul dens UI
            if (spawnedBomb != null && bombVisualsScript != null)
            {
                bombVisualsScript.HidePlantingUI();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlantZone"))
        {
            isInPlantZone = true;
            plantLocation = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlantZone"))
        {
            isInPlantZone = false;
        }
    }
}