using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EvaluationLayer : MonoBehaviour
{
    [SerializeField] private InputLayer _input;
    [Header("Transitions")]
    [SerializeField] private EvaluationTransition _directTransition;
    [SerializeField] private EvaluationTransition _futhureTransition;
    [SerializeField] private EvaluationTransition _secondaryTransition;
    [SerializeField] private EvaluationTransition _ethicsTransition;
    [Header("Output")]
    [SerializeField] private TextMeshProUGUI _directRawField;
    [SerializeField] private TextMeshProUGUI _directValField;
    private float _directRaw;
    private float _directVal;
    [Space]
    [SerializeField] private TextMeshProUGUI _futhureRawField;
    [SerializeField] private TextMeshProUGUI _futhureValField;
    private float _futhureRaw;
    private float _futhureVal;
    [Space]
    [SerializeField] private TextMeshProUGUI _secondaryRawField;
    [SerializeField] private TextMeshProUGUI _secondaryValField;
    private float _secondaryRaw;
    private float _secondaryVal;
    [Space]
    [SerializeField] private TextMeshProUGUI _ethicsRawField;
    [SerializeField] private TextMeshProUGUI _ethicsValField;
    private float _ethicsRaw;
    private float _ethicsVal;

    private EvaluationOption[] _directOptions;
    private EvaluationOption[] _futhureOptions;
    private EvaluationOption[] _secondaryOptions;
    private EvaluationOption[] _ethicsOptions;

    public EvaluationOption[] DirectOptions => _directOptions;
    public EvaluationOption[] FuthureOptions => _futhureOptions;
    public EvaluationOption[] SecondaryOptions => _secondaryOptions;
    public EvaluationOption[] EthicsOptions => _ethicsOptions;

    public EasingType DirectEasing { get; set; }
    public EasingType FuthureEasing { get; set; }
    public EasingType SecondaryEasing { get; set; }
    public EasingType EthicsEasing { get; set; }

    private void Awake()
    {
        _directOptions = GenerateOptions(_directTransition);
        _futhureOptions = GenerateOptions(_futhureTransition);
        _secondaryOptions = GenerateOptions(_secondaryTransition);
        _ethicsOptions = GenerateOptions(_ethicsTransition);
    }

    private void Start()
    {
        _input.OnInputChanged += OnInputChanged;
    }

    private void OnDestroy()
    {
        _input.OnInputChanged -= OnInputChanged;
    }

    private EvaluationOption[] GenerateOptions(EvaluationTransition transition)
    {
        List<EvaluationOption> optionList = new(4);
        foreach (var type in transition.GetTransition())
        {
            optionList.Add(EvaluationOption.CreateInstance(type));
        }

        return optionList.ToArray();
    }

    private void OnInputChanged(InputChangedArgs args)
    {
        _directRaw = CalculateRaw(DirectOptions, args);
        _futhureRaw = CalculateRaw(FuthureOptions, args);
        _secondaryRaw = CalculateRaw(SecondaryOptions, args);
        _ethicsRaw = CalculateRaw(EthicsOptions, args);

        _directVal = DirectEasing.Evaluate(Mathf.InverseLerp(0.0f, 4.0f, _directRaw));
        _futhureVal = FuthureEasing.Evaluate(Mathf.InverseLerp(0.0f, 4.0f, _futhureRaw));
        _secondaryVal = SecondaryEasing.Evaluate(Mathf.InverseLerp(0.0f, 4.0f, _secondaryRaw));
        _ethicsVal = EthicsEasing.Evaluate(Mathf.InverseLerp(0.0f, 4.0f, _ethicsRaw));

        UpdateUI();
    }

    private void UpdateUI()
    {
        _directRawField.SetText(_directRaw.ToString("0.000"));
        _directValField.SetText(_directVal.ToString("0.000"));
        _futhureRawField.SetText(_futhureRaw.ToString("0.000"));
        _futhureValField.SetText(_futhureVal.ToString("0.000"));
        _secondaryRawField.SetText(_secondaryRaw.ToString("0.000"));
        _secondaryValField.SetText(_secondaryVal.ToString("0.000"));
        _ethicsRawField.SetText(_ethicsRaw.ToString("0.000"));
        _ethicsValField.SetText(_ethicsVal.ToString("0.000"));
    }

    private float CalculateRaw(EvaluationOption[] options, InputChangedArgs args)
    {
        return options[0].Evaluate(args.inputOneValue)
            + options[1].Evaluate(args.inputTwoValue)
            + options[2].Evaluate(args.inputThreeValue)
            + options[3].Evaluate(args.inputFourValue);
    }

    [System.Serializable]
    private class EvaluationTransition
    {
        [SerializeField] private EvaluationType _fromInputOne;
        [SerializeField] private EvaluationType _fromInputTwo;
        [SerializeField] private EvaluationType _fromInputThree;
        [SerializeField] private EvaluationType _fromInputFour;

        public IEnumerable<EvaluationType> GetTransition()
        {
            yield return _fromInputOne;
            yield return _fromInputTwo;
            yield return _fromInputThree;
            yield return _fromInputFour;
        }
    }
}
