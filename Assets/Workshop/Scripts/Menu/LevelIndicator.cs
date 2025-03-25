using Graph;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

[Serializable]
public class LevelIndicator
{
    [SerializeField] private TextMeshProUGUI _titelField;
    [SerializeField] private TextMeshProUGUI _valueField;
    [SerializeField] private Image _fillImage;

    public float maxValue = -1f;

    public void SetTitel(string titel)
    { 
        _titelField.text = titel;
    }

    public void SetValue(float value, string format = "0.00")
    {
        _valueField.text = value.ToString(format: format);

        if(maxValue > 0f)
        {
            _fillImage.fillAmount = value / maxValue;
        }
    }
}

[Serializable]
public class LevelRegulator
{
    [SerializeField] private TextMeshProUGUI _titelField;
    [SerializeField] private TextMeshProUGUI _valueField;
    [SerializeField] private Slider _levelSlider;

    public delegate void ValueChanged(SO_GraphNode node, int newValue);
    public event ValueChanged OnValueChanged;

    private SO_GraphNode _node;

    public void Init(SO_GraphNode node, float maxValue)
    {
        _node = node;
        _levelSlider.maxValue = maxValue;
        _levelSlider.wholeNumbers = true;
        _levelSlider.onValueChanged.AddListener(OnSliderChanged);

        _titelField.text = node.description;
        SetSlider(0f);
        SetLabel(0f);
    }

    public void Deactivate()
    {
        _levelSlider.onValueChanged.RemoveListener(OnSliderChanged);
    }

    public void UpdateUI()
    {
        float value = _node.GetValue();
        SetLabel(value);
    }

    private void OnSliderChanged(float value)
    {
        OnValueChanged?.Invoke(_node, (int)value);
    }

    public void SetLabel(float value, string format = "0.00")
    {
        _valueField.text = value.ToString(format: format);
    }

    public void SetSlider(float value)
    {
        _levelSlider.SetValueWithoutNotify(value);
    }

    public float GetSliderValue()
    {
        return _levelSlider.value;
    }
}
