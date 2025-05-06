using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerUI : MonoBehaviour
{
    private enum DisplayState
    {
        None,
        Direct,
        Futhure,
        Secondary,
        Ethics,
    }

    [SerializeField] private GameManager _gameManager;
    [Header("Navigation")]
    [SerializeField] private Button _direct;
    [SerializeField] private Button _futhure;
    [SerializeField] private Button _secondary;
    [SerializeField] private Button _ethics;
    [Space]
    [SerializeField] private Button _close;
    [Header("UI")]
    [SerializeField] private GameObject _panel;
    [SerializeField] private FloatRangeUI _slider;
    [SerializeField] private TextMeshProUGUI _valueField;

    private DisplayState _currentState = DisplayState.None;

    private void Start()
    {
        _direct.onClick.AddListener(OnDirectClicked);
        _futhure.onClick.AddListener(OnFuthureClicked);
        _secondary.onClick.AddListener(OnSecondaryClicked);
        _ethics.onClick.AddListener(OnEthicsClicked);
        _close.onClick.AddListener(OnCloseClicked);
        _slider.valueSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnDestroy()
    {
        _direct.onClick.RemoveListener(OnDirectClicked);
        _futhure.onClick.RemoveListener(OnFuthureClicked);
        _secondary.onClick.RemoveListener(OnSecondaryClicked);
        _ethics.onClick.RemoveListener(OnEthicsClicked);
        _close.onClick.RemoveListener(OnCloseClicked);
        _slider.valueSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
    }

    private void OnDirectClicked()
    {
        OpenSettings(DisplayState.Direct, _gameManager.DirectBreakpoint, 0.0f, 1.0f, false);
    }

    private void OnFuthureClicked()
    {
        OpenSettings(DisplayState.Futhure, _gameManager.FuthureMulti, 1.0f, 4.0f, true);
    }

    private void OnSecondaryClicked()
    {
        OpenSettings(DisplayState.Secondary, _gameManager.SecondaryContinue, 0.0f, 1.0f, false);
    }

    private void OnEthicsClicked()
    {
        OpenSettings(DisplayState.Ethics, _gameManager.EthicsMulti, 0.5f, 2.0f, false);
    }

    private void OpenSettings(DisplayState state, float value, float min, float max, bool useWholeNumbers)
    {
        _panel.SetActive(true);
        _currentState = state;

        _slider.start.text = min.ToString();
        _slider.end.text = max.ToString();
        _valueField.text = value.ToString("0.00");

        _slider.valueSlider.minValue = min;
        _slider.valueSlider.maxValue = max;
        _slider.valueSlider.SetValueWithoutNotify(value);
        _slider.valueSlider.wholeNumbers = useWholeNumbers;
    }

    private void OnSliderValueChanged(float value)
    {
        _valueField.text = value.ToString("0.00");
        switch(_currentState)
        {
            case DisplayState.Direct:
                _gameManager.DirectBreakpoint = value;
                break;
            case DisplayState.Futhure:
                _gameManager.FuthureMulti = value;
                break;
            case DisplayState.Secondary:
                _gameManager.SecondaryContinue = value;
                break;
            case DisplayState.Ethics:
                _gameManager.EthicsMulti = value;
                break;

            case DisplayState.None:
            default:
                throw new NotImplementedException();
        }
    }

    private void OnCloseClicked()
    {
        _currentState = DisplayState.None;
        _panel.SetActive(false);
    }
}
