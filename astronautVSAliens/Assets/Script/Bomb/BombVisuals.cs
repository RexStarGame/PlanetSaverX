using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BombVisuals : MonoBehaviour
{
    [Header("Planting Bar")]
    // Brug den SAMME image som senere bliver health bar
    public Image plantingFillImage;

    [Header("Valgfri elementer under planting")]
    public Image plantingBackgroundImage;

    // Denne tekst skal skjules mens man loader bomben
    public TMP_Text countdownTextToHide;

    public void ShowPlantingUI()
    {
        if (plantingFillImage != null)
        {
            plantingFillImage.enabled = true;
        }

        if (plantingBackgroundImage != null)
        {
            plantingBackgroundImage.enabled = true;
        }

        // Skjul countdown tekst mens vi planter
        if (countdownTextToHide != null)
        {
            countdownTextToHide.enabled = false;
        }
    }

    public void HidePlantingUI()
    {
        // Vi skjuler IKKE selve canvas
        // Vi skjuler kun planting-udseendet
        if (plantingBackgroundImage != null)
        {
            plantingBackgroundImage.enabled = false;
        }

        // Vi skjuler ikke plantingFillImage her permanent,
        // fordi Bomb.cs lige bagefter bruger samme image som health bar.
        // Derfor lader vi den være enabled.
    }

    public void UpdateVisuals(float progressPercent)
    {
        if (plantingFillImage != null)
        {
            plantingFillImage.fillAmount = Mathf.Clamp01(progressPercent);
        }
    }
}
