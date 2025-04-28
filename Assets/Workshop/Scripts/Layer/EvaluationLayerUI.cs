using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EvaluationLayerUI : MonoBehaviour
{
    private enum DisplayState
    {
        None,
        Direct,
        Futhure,
        Secondary,
        Ethics,
    }

    [SerializeField] private EvaluationLayer _layer;
    [Header("Navigation")]
    [SerializeField] private Button _directOptions;
    [SerializeField] private Button _futhureOptions;
    [SerializeField] private Button _secondaryOptions;
    [SerializeField] private Button _ethicsOptions;
    [Space]
    [SerializeField] private Button _close;
    [Header("Evaluation UI")]
    [SerializeField] private GameObject _ui;
    [Space]
    [SerializeField] private EvaluationOptionUI _inputOne;
    [SerializeField] private EvaluationOptionUI _inputTwo;
    [SerializeField] private EvaluationOptionUI _inputThree;
    [SerializeField] private EvaluationOptionUI _inputFour;

    private DisplayState _activeState;

    private void Start()
    {
        _inputOne.Init();
        _inputOne.OptionChanged += (opt) => GetEvaluationOptions()[0] = opt;

        _inputTwo.Init();
        _inputTwo.OptionChanged += (opt) => GetEvaluationOptions()[1] = opt;

        _inputThree.Init();
        _inputThree.OptionChanged += (opt) => GetEvaluationOptions()[2] = opt;

        _inputFour.Init();
        _inputFour.OptionChanged += (opt) => GetEvaluationOptions()[3] = opt;

        _directOptions.onClick.AddListener(OnDirectClicked);
        _futhureOptions.onClick.AddListener(OnFuthureClicked);
        _secondaryOptions.onClick.AddListener(OnSecondaryClicked);
        _ethicsOptions.onClick.AddListener(OnEthicsClicked);
        _close.onClick.AddListener(OnCloseClicked);
    }

    private void OnDestroy()
    {
        _directOptions.onClick.RemoveListener(OnDirectClicked);
        _futhureOptions.onClick.RemoveListener(OnFuthureClicked);
        _secondaryOptions.onClick.RemoveListener(OnSecondaryClicked);
        _ethicsOptions.onClick.RemoveListener(OnEthicsClicked);
        _close.onClick.RemoveListener(OnCloseClicked);
    }

    private void OnDirectClicked()
    {
        SetOptionUIs(DisplayState.Direct);
    }

    private void OnFuthureClicked()
    {
        SetOptionUIs(DisplayState.Futhure);
    }

    private void OnSecondaryClicked()
    {
        SetOptionUIs(DisplayState.Secondary);
    }

    private void OnEthicsClicked()
    {
        SetOptionUIs(DisplayState.Ethics);
    }

    private void SetOptionUIs(DisplayState activeState)
    {
        _ui.SetActive(true);
        _activeState = activeState;

        var options = GetEvaluationOptions();
        _inputOne.SetData(options[0]);
        _inputTwo.SetData(options[1]);
        _inputThree.SetData(options[2]);
        _inputFour.SetData(options[3]);
    }

    private EvaluationOption[] GetEvaluationOptions()
    {
        return _activeState switch
        {
            DisplayState.Direct => _layer.DirectOptions,
            DisplayState.Futhure => _layer.FuthureOptions,
            DisplayState.Secondary => _layer.SecondaryOptions,
            DisplayState.Ethics => _layer.EthicsOptions,
            _ => throw new NotImplementedException(),
        };
    }

    private void OnCloseClicked()
    {
        _ui.SetActive(false);
        _activeState = DisplayState.None;
    }

    [Serializable]
    private class EvaluationOptionUI
    {
        [SerializeField] private TMP_Dropdown _type;
        [SerializeField] private UiLineRenderer _visual;
        [SerializeField] private Transform _varContainer;
        [SerializeField] private FloatRangeUI _varTemplate;
        [SerializeField] private Button _remove;
        [SerializeField] private Button _add;

        public event Action<EvaluationOption> OptionChanged;

        private EvaluationOption _evaluationData;
        private List<FloatRangeUI> _variables;

        public void Init()
        {
            _variables = new(5);
            _type.onValueChanged.AddListener(OnTypeChanged);
            _remove.onClick.AddListener(OnRemoveClicked);
            _add.onClick.AddListener(OnAddClicked);
        }

        public void SetData(EvaluationOption evaluationOption)
        {
            if (_variables == null)
            {
                Debug.LogWarning("Call init befor setting data.");
                return;
            }

            _evaluationData = evaluationOption ?? throw new ArgumentNullException();

            //Set Type
            int idx = _evaluationData.GetEvaluationType() switch
            {
                EvaluationType.Linear => 0,
                EvaluationType.Step => 1,
                EvaluationType.EaseInOut => 2,
                EvaluationType.Sin => 3,
                _ => throw new NotImplementedException(),
            };
            _type.SetValueWithoutNotify(idx);

            //Update visuals
            UpdateVariables();
        }

        private void OnTypeChanged(int idx)
        {
            if (Enum.TryParse(_type.options[idx].text, true, out EvaluationType type))
            {
                var newData = EvaluationOption.CreateInstance(type);
                SetData(newData);
                OptionChanged?.Invoke(newData);
            }
            else
            {
                Debug.LogWarning($"Failed to parse: {_type.options[idx].text}");
            }
        }

        private void OnValueChanged(float val)
        {
            _evaluationData.SetVariables(
                _variables.Select(v => v.valueSlider.value).ToArray());
            _visual.PlotEvaluationOption(_evaluationData);
        }

        private void OnRemoveClicked()
        {
            _evaluationData.RemoveVariable();
            UpdateVariables();
        }

        private void OnAddClicked()
        {
            _evaluationData.AddVariable();
            UpdateVariables();
        }

        private void UpdateVariables()
        {
            foreach (var varGO in _variables.Select(v => v.gameObject))
            {
                Destroy(varGO);
            }
            _variables.Clear();

            foreach (var variable in _evaluationData.GetVariables())
            {
                var floatRangeUI = Instantiate(_varTemplate, _varContainer);
                floatRangeUI.start.SetText(variable.start.ToString());
                floatRangeUI.end.SetText(variable.end.ToString());

                if(variable.end > 1.0f)
                {
                    floatRangeUI.valueSlider.minValue = variable.start;
                    floatRangeUI.valueSlider.maxValue = variable.end;
                    floatRangeUI.valueSlider.wholeNumbers = true;
                }
                floatRangeUI.valueSlider.SetValueWithoutNotify(variable.value);
                floatRangeUI.valueSlider.onValueChanged.AddListener(OnValueChanged);

                floatRangeUI.gameObject.SetActive(true);
                _variables.Add(floatRangeUI);
            }

            _visual.PlotEvaluationOption(_evaluationData);

            _remove.interactable = _evaluationData.CanRemoveVariable;
            _add.interactable = _evaluationData.CanAddVariable;
        }
    }
}
