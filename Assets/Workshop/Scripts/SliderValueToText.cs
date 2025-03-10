using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderValueToText : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI textDisplay;
    [SerializeField] private int decimalPlaces = 0;
    [SerializeField] private bool wholeNumbersOnly = false;

    private void Start()
    {
        UpdateText(slider.value);

        slider.onValueChanged.AddListener(UpdateText);
    }

    private void UpdateText(float value)
    {
        string formattedValue;

        if (wholeNumbersOnly)
        {
            formattedValue = Mathf.RoundToInt(value).ToString();
        }
        else
        {
            string format = "F" + decimalPlaces.ToString();
            formattedValue = value.ToString(format);
        }

        textDisplay.text = formattedValue;
    }
}