using Graph;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputMenu : MonoBehaviour
{
    const int MAX_VALUE = 20;

    [Header("Inputs")]
    [SerializeField] private LevelRegulator _inputOne;
    [SerializeField] private LevelRegulator _inputTwo;
    [SerializeField] private LevelRegulator _inputThree;
    [SerializeField] private LevelRegulator _inputFour;
    [Header("Presets")]
    [SerializeField] private Button _presetBtnOne;
    [SerializeField] private Button _presetBtnTwo;
    [SerializeField] private Button _presetBtnThree;
    [Header("Preset Data")]
    [SerializeField] private Preset _presetOne;
    [SerializeField] private Preset _presetTwo;
    [SerializeField] private Preset _presetThree;
    [Header("Feedback")]
    [SerializeField] private Toggle _savePresetBtn;
    [SerializeField] private GameObject _feedbackContainer;
    [SerializeField] private TextMeshProUGUI _feedbackText;

    private bool _isSavingPreset;
    private WorkshopManager _manager;

    private bool _inputValuesChanged;
    private int _currentInputValueSum;
    private Dictionary<SO_GraphNode, float> _currentInputValues;
    private Dictionary<SO_GraphNode, LevelRegulator> _nodeRegulators;

    public float CurrentInputValueSum => (float) _currentInputValueSum / MAX_VALUE;
    public Dictionary<SO_GraphNode, float> CurrentInputValues => _currentInputValues;

    private void Start()
    {
        ToggleSaveMode(false);
        _currentInputValues = new(4);
        _nodeRegulators = new(4);
    }

    private void OnEnable()
    {
        _presetBtnOne.onClick.AddListener(() => SetPreset(_presetOne));
        _presetBtnTwo.onClick.AddListener(() => SetPreset(_presetTwo));
        _presetBtnThree.onClick.AddListener(() => SetPreset(_presetThree));

        _savePresetBtn.onValueChanged.AddListener(ToggleSaveMode);
    }

    private void OnDisable()
    {
        _presetBtnOne.onClick.RemoveAllListeners();
        _presetBtnTwo.onClick.RemoveAllListeners();
        _presetBtnThree.onClick.RemoveAllListeners();

        _savePresetBtn.onValueChanged.RemoveAllListeners();
    }

    private void LateUpdate()
    {
        if(_inputValuesChanged)
        {
            _manager.UpdateInputValues(_currentInputValues);
            _inputValuesChanged = false;
        }
    }

    private void OnDestroy()
    {
        _manager.OnGraphUpdated -= OnGraphUpdated;
    }

    private void OnGraphUpdated()
    {
        _inputOne.UpdateUI(_manager.UseNormalizedValue);
        _inputTwo.UpdateUI(_manager.UseNormalizedValue);
        _inputThree.UpdateUI(_manager.UseNormalizedValue);
        _inputFour.UpdateUI(_manager.UseNormalizedValue);
    }

    public void Init(WorkshopManager manager, SO_GraphNode[] startNodes)
    {
        _manager = manager;
        _manager.OnGraphUpdated += OnGraphUpdated;

        InitInput(startNodes[0], _inputOne);
        InitInput(startNodes[1], _inputTwo);
        InitInput(startNodes[2], _inputThree);
        InitInput(startNodes[3], _inputFour);
    }

    private void InitInput(SO_GraphNode node, LevelRegulator regulator)
    {
        regulator.Init(node, MAX_VALUE);
        regulator.OnValueChanged += OnInputValueChange;

        _nodeRegulators.Add(node, regulator);
        _currentInputValues.Add(node, 0f);

        _inputValuesChanged = true;
    }

    private void OnInputValueChange(SO_GraphNode node, int newValue)
    {
        _currentInputValueSum -= Mathf.RoundToInt(_currentInputValues[node] * MAX_VALUE);
        var ensuredVal = EnsureInputValue(newValue);
        if(ensuredVal != newValue)
        {
            var sliderValue = _currentInputValues[node] * MAX_VALUE;
            _nodeRegulators[node].SetSlider(sliderValue);
        }
        else
        {
            _currentInputValues[node] = (float) ensuredVal / MAX_VALUE;
            _inputValuesChanged = true;
        }
    }

    private float EnsureInputValue(int amoutToAdd)
    {
        if(_currentInputValueSum + amoutToAdd > MAX_VALUE)
        {
            var value = MAX_VALUE - _currentInputValueSum;
            _currentInputValueSum = MAX_VALUE;
            return value;
        }

        _currentInputValueSum += amoutToAdd;
        return amoutToAdd;
    }

    #region Presets
    private void ToggleSaveMode(bool active)
    {
        _isSavingPreset = active;
        _feedbackContainer.SetActive(active);

        if(_isSavingPreset)
        {
            _feedbackText.text = "Choose Preset to save.";
        }
    }

    private void SetPreset(Preset preset)
    {
        Debug.Log("Disabled for now.");
        return;

        if(_isSavingPreset)
        {
            StartCoroutine(SavePresetRoutine(preset));
        }
        else
        {
            _inputOne.SetSlider(preset.InputOne);
            _inputTwo.SetSlider(preset.InputTwo);
            _inputThree.SetSlider(preset.InputThree);
            _inputFour.SetSlider(preset.InputFour);
        }
    }

    private IEnumerator SavePresetRoutine(Preset preset)
    {
        preset.InputOne = _inputOne.GetSliderValue();
        preset.InputTwo = _inputTwo.GetSliderValue();
        preset.InputThree = _inputThree.GetSliderValue();
        preset.InputFour = _inputFour.GetSliderValue();

        _feedbackText.text = "Preset saved.";
        _savePresetBtn.SetIsOnWithoutNotify(false);

        yield return new WaitForSeconds(1f);

        ToggleSaveMode(false);
    }

    [Serializable]
    public class Preset
    {
        [Range(0, 1)] public float InputOne;
        [Range(0, 1)] public float InputTwo;
        [Range(0, 1)] public float InputThree;
        [Range(0, 1)] public float InputFour;
    }
    #endregion
}
