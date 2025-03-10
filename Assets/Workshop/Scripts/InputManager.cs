using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class InputManager : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider eurosSlider;
    [SerializeField] private Slider vollzeitSlider;
    [SerializeField] private Slider serverSlider;

  

    [Header("Preset Buttons")]
    [SerializeField] private Button preset1Button;
    [SerializeField] private Button preset2Button;
    [SerializeField] private Button preset3Button;
    [SerializeField] private Button saveButton;

    [Header("Feedback Elements")]
    [SerializeField] private GameObject feedbackObject;
    [SerializeField] private Image saveButtonImage;
    [SerializeField] private Color normalSaveColor = Color.white;
    [SerializeField] private Color activeSaveColor = Color.yellow;
    [SerializeField] private Color successColor = Color.green;

    // TMP als Kind des Feedback GameObjects
    private TextMeshProUGUI feedbackText;

    [System.Serializable]
    public class PresetValues
    {
        public float eurosValue = 0f;
        public float vollzeitValue = 0f;
        public float serverValue = 0f;
    }

    [Header("Preset Configurations")]
    [SerializeField] private PresetValues[] presets = new PresetValues[3];

    private int selectedPresetForSaving = -1; // -1 bedeutet keiner ausgewählt
    private bool isInSaveMode = false;

    void Awake()
    {
        for (int i = 0; i < 3; i++)
        {
            if (presets[i] == null)
                presets[i] = new PresetValues();
        }

        if (feedbackObject != null)
        {
            feedbackText = feedbackObject.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    void Start()
    {
        preset1Button.onClick.AddListener(() => HandlePresetButtonClick(0));
        preset2Button.onClick.AddListener(() => HandlePresetButtonClick(1));
        preset3Button.onClick.AddListener(() => HandlePresetButtonClick(2));

        saveButton.onClick.AddListener(ToggleSaveMode);

       

        if (feedbackObject != null)
        {
            feedbackObject.SetActive(false);
        }
    }

    private void HandlePresetButtonClick(int presetIndex)
    {
        if (isInSaveMode)
        {
            SaveCurrentValuesToPreset(presetIndex);
            ExitSaveMode();
            ShowSaveSuccessEffect(presetIndex);
        }
        else
        {
            LoadPreset(presetIndex);
        }
    }

    public void LoadPreset(int presetIndex)
    {
        eurosSlider.value = presets[presetIndex].eurosValue;
        vollzeitSlider.value = presets[presetIndex].vollzeitValue;
        serverSlider.value = presets[presetIndex].serverValue;

    }

    private void ToggleSaveMode()
    {
        isInSaveMode = !isInSaveMode;
        HighlightSaveMode(isInSaveMode);

        if (isInSaveMode)
        {
            ShowFeedbackText("Wähle Preset 1, 2 oder 3 zum Speichern");
        }
        else
        {
            HideFeedbackText();
        }
    }

    private void EnterSaveMode()
    {
        isInSaveMode = true;
        HighlightSaveMode(true);
        ShowFeedbackText("Wähle Preset 1, 2 oder 3 zum Speichern");
    }

    private void ExitSaveMode()
    {
        isInSaveMode = false;
        HighlightSaveMode(false);
        HideFeedbackText();
    }

    private void HighlightSaveMode(bool isActive)
    {
        if (saveButtonImage != null)
        {
            saveButtonImage.color = isActive ? activeSaveColor : normalSaveColor;
        }

        Color saveColor = isActive ? activeSaveColor : Color.white;
        preset1Button.GetComponent<Image>().color = saveColor;
        preset2Button.GetComponent<Image>().color = saveColor;
        preset3Button.GetComponent<Image>().color = saveColor;
    }

    private void ShowSaveSuccessEffect(int presetIndex)
    {
        Button presetButton = null;

        switch (presetIndex)
        {
            case 0: presetButton = preset1Button; break;
            case 1: presetButton = preset2Button; break;
            case 2: presetButton = preset3Button; break;
        }

        if (presetButton != null)
        {
            Image buttonImage = presetButton.GetComponent<Image>();
            Color originalColor = buttonImage.color;

            ShowFeedbackText($"Preset {presetIndex + 1} gespeichert!");

            StartCoroutine(FlashButtonSuccess(buttonImage, originalColor));
        }
    }

    private IEnumerator FlashButtonSuccess(Image buttonImage, Color originalColor)
    {
        for (int i = 0; i < 2; i++)
        {
            buttonImage.color = successColor;
            yield return new WaitForSeconds(0.15f);
            buttonImage.color = originalColor;
            yield return new WaitForSeconds(0.15f);
        }

        yield return new WaitForSeconds(0.5f);
        HideFeedbackText();
    }

    private void ShowFeedbackText(string message)
    {
        if (feedbackObject != null)
        {
            if (feedbackText != null)
            {
                feedbackText.text = message;
            }
            feedbackObject.SetActive(true);
        }
    }

    private void HideFeedbackText()
    {
        if (feedbackObject != null)
        {
            feedbackObject.SetActive(false);
        }
    }
    private void SaveCurrentValuesToPreset(int presetIndex)
    {
        presets[presetIndex].eurosValue = eurosSlider.value;
        presets[presetIndex].vollzeitValue = vollzeitSlider.value;
        presets[presetIndex].serverValue = serverSlider.value;
    }

    

}