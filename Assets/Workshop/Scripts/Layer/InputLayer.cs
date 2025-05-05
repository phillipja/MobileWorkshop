using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputLayer : MonoBehaviour
{
    [SerializeField] private InputSlider _inputOne;
    [SerializeField] private InputSlider _inputTwo;
    [SerializeField] private InputSlider _inputThree;
    [SerializeField] private InputSlider _inputFour;
    [Space]
    [SerializeField, Range(1, 4)] private int _maxValueSum = 1;
    [Space]
    [SerializeField] private Button _presetOneButton;
    [SerializeField] private Button _presetTwoButton;
    [SerializeField] private Button _saveButton;
    [SerializeField] private Button _randomButton;
    [Space]
    [SerializeField] private GameObject _notificationPanel;
    [SerializeField] private TextMeshProUGUI _notificationText;
    [SerializeField] private float _notificationDuration = 2f;
    [Space]
    [Header("Preset 1 Werte")]
    [SerializeField, Range(0, 1)] private float _preset1Value1;
    [SerializeField, Range(0, 1)] private float _preset1Value2;
    [SerializeField, Range(0, 1)] private float _preset1Value3;
    [SerializeField, Range(0, 1)] private float _preset1Value4;
    [Space]
    [Header("Preset 2 Werte")]
    [SerializeField, Range(0, 1)] private float _preset2Value1;
    [SerializeField, Range(0, 1)] private float _preset2Value2;
    [SerializeField, Range(0, 1)] private float _preset2Value3;
    [SerializeField, Range(0, 1)] private float _preset2Value4;

    private int _selectedPreset;

    private Coroutine _notificationCoroutine;

    public delegate void InputChanged(InputChangedArgs args);
    public event InputChanged OnInputChanged;

    private bool _inputChanged;
    private float _valueSum;

    void Start()
    {
        foreach (var input in GetSlider())
        {
            input.valueChanged = EnsureValue;
        }

        _presetOneButton.onClick.AddListener(() => LoadPreset(1));
        _presetTwoButton.onClick.AddListener(() => LoadPreset(2));
        _saveButton.onClick.AddListener(SavePreset);
        _randomButton.onClick.AddListener(SetRandomValues); 

        if (_notificationPanel != null)
        {
            _notificationPanel.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (_inputChanged)
        {
            OnInputChanged?.Invoke(new()
            {
                valueSumLevel = _valueSum / _maxValueSum,
                inputOneValue = _inputOne.Value,
                inputTwoValue = _inputTwo.Value,
                inputThreeValue = _inputThree.Value,
                inputFourValue = _inputFour.Value,
            });

            _inputChanged = false;
        }
    }

    private void OnDestroy()
    {
        foreach (var input in GetSlider())
        {
            input.valueChanged = null;
        }

        _presetOneButton.onClick.RemoveListener(() => LoadPreset(1));
        _presetTwoButton.onClick.RemoveListener(() => LoadPreset(2));
        _saveButton.onClick.RemoveListener(SavePreset);
        _randomButton.onClick.RemoveListener(SetRandomValues);
    }

    private float EnsureValue(float oldVal, float newVal)
    {
        _valueSum -= oldVal;
        _valueSum += newVal;

        _inputChanged = oldVal != newVal;
        return newVal;

        //if (newVal < oldVal)
        //{
        //    _valueSum += newVal;
        //    _inputChanged = true;
        //    return newVal;
        //}
        //else
        //{
        //    float ensuredVal = Mathf.Min(newVal, _maxValueSum - _valueSum);
        //    _valueSum += ensuredVal;
        //    _inputChanged = oldVal - ensuredVal != 0f;
        //    return ensuredVal;
        //}
    }

    public void SetRandomValues()
    {
        var sliders = GetSliderArray();

        _valueSum = 0;

        for (int i = 0; i < sliders.Length; i++)
        {
            var temp = sliders[i].valueChanged;
            sliders[i].valueChanged = null;

            float randomValue = Mathf.Round(Random.Range(0, 21)) * 0.05f;
            sliders[i].SetValue(randomValue);
            sliders[i].valueChanged = temp;

            _valueSum += sliders[i].Value;
        }

        _inputChanged = true;
    }

    public void ResetInput()
    {
        foreach (var input in GetSlider())
        {
            input.ResetValue();
        }
        _valueSum = 0f;
    }

    private IEnumerable<InputSlider> GetSlider()
    {
        yield return _inputOne;
        yield return _inputTwo;
        yield return _inputThree;
        yield return _inputFour;
    }

    public void LoadPreset(int presetNumber)
    {
        var sliders = GetSliderArray();

        if (_notificationPanel != null && _notificationPanel.activeSelf && _notificationText.text == "Bitte auf einen Preset-Button klicken zum Speichern")
        {
            float[] currentValues = new float[4];
            for (int i = 0; i < sliders.Length; i++)
            {
                currentValues[i] = sliders[i].Value;
            }

            SetPresetValues(presetNumber, currentValues);

            ShowNotification("Erfolgreich gespeichert", true);
            return;
        }

        _selectedPreset = presetNumber;

        float[] valuesToLoad = GetPresetValues(presetNumber);

        _valueSum = 0;

        for (int i = 0; i < sliders.Length; i++)
        {
            var temp = sliders[i].valueChanged;
            sliders[i].valueChanged = null;
            sliders[i].SetValue(valuesToLoad[i]);
            sliders[i].valueChanged = temp;

            _valueSum += sliders[i].Value;
        }

        _inputChanged = true;
    }

    public void SavePreset()
    {
        ShowNotification("Bitte auf einen Preset-Button klicken zum Speichern", false);
    }

    private void ShowNotification(string message, bool useTimer)
    {
        if (_notificationPanel == null || _notificationText == null) return;

        if (_notificationCoroutine != null)
        {
            StopCoroutine(_notificationCoroutine);
            _notificationCoroutine = null;
        }

        _notificationText.text = message;
        _notificationPanel.SetActive(true);

        if (useTimer)
        {
            _notificationCoroutine = StartCoroutine(HideNotificationAfterDelay());
        }
    }

    private IEnumerator HideNotificationAfterDelay()
    {
        yield return new WaitForSeconds(_notificationDuration);
        _notificationPanel.SetActive(false);
        _notificationCoroutine = null;
        _selectedPreset = 0;
    }
    private void RecalculateSum()
    {
        _valueSum = 0;
        foreach (var slider in GetSlider())
        {
            _valueSum += slider.Value;
        }
    }

    private InputSlider[] GetSliderArray()
    {
        return new[] { _inputOne, _inputTwo, _inputThree, _inputFour };
    }

    private float[] GetPresetValues(int presetNumber)
    {
        if (presetNumber == 1)
        {
            return new float[] { _preset1Value1, _preset1Value2, _preset1Value3, _preset1Value4 };
        }
        else
        {
            return new float[] { _preset2Value1, _preset2Value2, _preset2Value3, _preset2Value4 };
        }
    }

    private void SetPresetValues(int presetNumber, float[] values)
    {
        if (presetNumber == 1)
        {
            _preset1Value1 = values[0];
            _preset1Value2 = values[1];
            _preset1Value3 = values[2];
            _preset1Value4 = values[3];
        }
        else
        {
            _preset2Value1 = values[0];
            _preset2Value2 = values[1];
            _preset2Value3 = values[2];
            _preset2Value4 = values[3];
        }
    }
}

public struct InputChangedArgs
{
    public float valueSumLevel;
    public float inputOneValue;
    public float inputTwoValue;
    public float inputThreeValue;
    public float inputFourValue;
}