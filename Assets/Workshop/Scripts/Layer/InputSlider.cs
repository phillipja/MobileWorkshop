using System;
using System.Collections;
using System.Collections.Generic;
using Graph;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputSlider : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _labelField;
    [SerializeField] private TextMeshProUGUI _valueField;
    [SerializeField] private Slider _inputSlider;

    public delegate float ValueChanged(float oldValue, float newValue);
    public ValueChanged valueChanged;

    public float Value { get; private set; }

    private void Start()
    {
        _inputSlider.onValueChanged.AddListener(OnSliderChanged);
    }

    private void OnDestroy()
    {
        _inputSlider.onValueChanged.RemoveListener(OnSliderChanged);
    }

    public void SetLabel(string text)
    {
        _labelField.SetText(text);
    }

    private void OnSliderChanged(float newValue)
    {
        if(_inputSlider.maxValue > 1)
        {
            newValue /= _inputSlider.maxValue;
        }

        float? ensuredValue = valueChanged?.Invoke(Value, newValue);
        Value = ensuredValue ?? newValue;
        _inputSlider.SetValueWithoutNotify(Value * _inputSlider.maxValue);
        _valueField.SetText(Value.ToString("0.00"));
    }

    public void SetValue(float val)
    {
        _inputSlider.SetValueWithoutNotify(val * _inputSlider.maxValue);
    }
}
