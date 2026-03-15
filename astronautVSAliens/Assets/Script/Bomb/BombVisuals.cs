using UnityEngine;
using UnityEngine.UI;

public class BombVisuals : MonoBehaviour
{
    private Image plantingProgressBar;
    private GameObject plantingUIContainer;

    void Awake()
    {
        // 1. Vi henter Bomb-scriptet, som allerede sidder på denne samme bombe
        Bomb bombScript = GetComponent<Bomb>();

        // 2. Vi spørger: "Hey Bomb, må jeg låne din timerFillImage?"
        if (bombScript != null && bombScript.timerFillImage != null)
        {
            // NU er vi 100% sikre på, at begge scripts bruger PRÆCIS det samme billede!
            plantingProgressBar = bombScript.timerFillImage;

            // Vi finder også det Canvas (forælderen), den ligger i, så vi kan tænde/slukke
            plantingUIContainer = plantingProgressBar.transform.parent.gameObject;
        }
    }

    void Start()
    {
        // Skjuler baren fra start, så den er klar til at playeren trykker 'E'
        HidePlantingUI();
    }

    public void ShowPlantingUI()
    {
        if (plantingUIContainer != null) plantingUIContainer.SetActive(true);
        else if (plantingProgressBar != null) plantingProgressBar.gameObject.SetActive(true);
    }

    public void HidePlantingUI()
    {
        if (plantingUIContainer != null) plantingUIContainer.SetActive(false);
        else if (plantingProgressBar != null) plantingProgressBar.gameObject.SetActive(false);
    }

    public void UpdateVisuals(float progressPercent)
    {
        if (plantingProgressBar != null)
        {
            // Her fylder vi baren fra 0 til 1 (0% til 100%)
            plantingProgressBar.fillAmount = progressPercent;
        }
    }
}