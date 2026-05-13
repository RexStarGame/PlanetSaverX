using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BombVisuals : MonoBehaviour
{
    [Header("Planting Bar")]
    public Image plantingFillImage;

    [Header("Valgfri elementer under planting")]
    public Image plantingBackgroundImage;

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
        if (countdownTextToHide != null)
        {
            countdownTextToHide.enabled = false;
        }
    }

    public void HidePlantingUI()
    {
        if (plantingBackgroundImage != null)
        {
            plantingBackgroundImage.enabled = false;
        }
    }

    public void UpdateVisuals(float progressPercent)
    {
        if (plantingFillImage != null)
        {
            plantingFillImage.fillAmount = Mathf.Clamp01(progressPercent);
        }
    }
}