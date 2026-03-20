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
    private Bomb activeBombLogic;

    private bool isInPlantZone = false;
    private bool isBombFullyPlanted = false;
    private Transform plantLocation;

    void Update()
    {
        if (isBombFullyPlanted) return;

        if (isInPlantZone && Input.GetKey(KeyCode.E))
        {
            if (spawnedBomb == null)
            {
                spawnedBomb = Instantiate(bombPrefab, plantLocation.position, Quaternion.identity);

                // Finder scripts både på root og children
                bombVisualsScript = spawnedBomb.GetComponentInChildren<BombVisuals>(true);
                activeBombLogic = spawnedBomb.GetComponentInChildren<Bomb>(true);

                if (bombVisualsScript == null)
                    Debug.LogError("Kunne ikke finde BombVisuals på bomben.");

                if (activeBombLogic == null)
                    Debug.LogError("Kunne ikke finde Bomb på bomben.");
            }

            currentProgress += Time.deltaTime;
            float progressPercent = Mathf.Clamp01(currentProgress / timeToPlant);

            if (bombVisualsScript != null)
            {
                bombVisualsScript.ShowPlantingUI();
                bombVisualsScript.UpdateVisuals(progressPercent);
            }

            if (currentProgress >= timeToPlant)
            {
                currentProgress = timeToPlant;
                isBombFullyPlanted = true;

                if (bombVisualsScript != null)
                    bombVisualsScript.HidePlantingUI();

                if (activeBombLogic != null)
                    activeBombLogic.ArmBomb();
            }
        }
        else
        {
            if (spawnedBomb != null && !isBombFullyPlanted && bombVisualsScript != null)
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