using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EvaluationLayer : MonoBehaviour
{
    private const float DIREKT_START_MULTI = 1.25992104989f;

    [SerializeField] private InputLayer _input;
    [Header("Transitions")]
    [SerializeField] private EvaluationTransition _directTransition;
    [SerializeField] private EvaluationTransition _futhureTransition;
    [SerializeField] private EvaluationTransition _secondaryTransition;
    [SerializeField] private EvaluationTransition _ethicsTransition;
    [Header("Round")]
    [SerializeField] private TextMeshProUGUI _roundLable;
    [SerializeField] private TextMeshProUGUI _budget;
    [SerializeField] private TextMeshProUGUI _return;
    [SerializeField] private Button _endRoundButton;
    [SerializeField] private GameObject _endScreen;
    [SerializeField] private TextMeshProUGUI _outro;
    [SerializeField] private Button _continue;
    [SerializeField] private Button _restart;
    private float _startBudget = 15.0f;
    private float _currentBudget = 15.0f;
    private float _nextBudget = 0.0f;
    private float _prevFuthureVal = 1.0f;
    private int _round = 1;
    [Header("Output")]
    [SerializeField] private TextMeshProUGUI _directRawField;
    [SerializeField] private TextMeshProUGUI _directMapField;
    [SerializeField] private TextMeshProUGUI _directValField;
    private float _directRaw;
    private float _directMap;
    private float _directVal;
    [Space]
    [SerializeField] private TextMeshProUGUI _futhureRawField;
    [SerializeField] private TextMeshProUGUI _futhureMapField;
    [SerializeField] private TextMeshProUGUI _futhureValField;
    private float _futhureRaw;
    private float _futhureMap;
    private float _futhureVal;
    [Space]
    [SerializeField] private TextMeshProUGUI _secondaryRawField;
    [SerializeField] private TextMeshProUGUI _secondaryMapField;
    [SerializeField] private TextMeshProUGUI _secondaryValField;
    private float _secondaryRaw;
    private float _secondaryMap;
    private float _secondaryVal;
    [Space]
    [SerializeField] private TextMeshProUGUI _ethicsRawField;
    [SerializeField] private TextMeshProUGUI _ethicsMapField;
    [SerializeField] private TextMeshProUGUI _ethicsValField;
    private float _ethicsRaw;
    private float _ethicsMap;
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

        _endRoundButton.onClick.AddListener(EndRound);
        _continue.onClick.AddListener(Continue);
        _restart.onClick.AddListener(RestartWorkshop);
    }

    private void OnDestroy()
    {
        _input.OnInputChanged -= OnInputChanged;
        _endRoundButton.onClick.RemoveListener(EndRound);
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

    private void EndRound()
    {
        var r = Random.Range(0f, 1f);

        if(r <= _secondaryVal)
        {
            var bonus = (10 * _round * _round) * _ethicsVal;
            _nextBudget += bonus;

            if(bonus > 0)
            {
                _outro.SetText($"You have been rewarded with {bonus:0.0€} for your ethical decisions." +
                    $"\n\nYour final balance for this round is {_nextBudget:0.0€}.");
            }
            else
            {
                if(_nextBudget > 0)
                {
                    _outro.SetText($"You have been punished with {bonus:0.0€} for your unethical decisions." +
                        $"\n\nYour final balance for this round is {_nextBudget:0.0€}.");
                }
                else
                {
                    _outro.SetText($"You have been punished with {bonus:0.0€} for your unethical decisions." +
                        $"\n\nYou bankrupted the company by your decisions. Final balance: {_nextBudget:0.0€}.");

                    _continue.gameObject.SetActive(false);
                    _restart.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            _outro.SetText($"Your decisions have been too insignificant. The public has not become aware of you." +
                    $"\n\nYour final return for this round is {_nextBudget:0.0€}.");
        }

        _startBudget = _nextBudget;
        _prevFuthureVal = _futhureVal;

        _endScreen.SetActive(true);
    }

    private void Continue()
    {
        _round++;
        _roundLable.SetText(_round.ToString());

        if(_round > 2)
        {
            var r = Random.Range(0f, 1f);
            float probability = 1.0f / Mathf.Pow(_round - 2, 2);
            bool continueGame = r < probability;
            _continue.gameObject.SetActive(continueGame);
            _restart.gameObject.SetActive(!continueGame);
        }

        PrepareRound();
    }

    private void PrepareRound()
    {
        _directRaw = _directMap = _directVal = 0.0f;
        _futhureRaw = _futhureMap = _futhureVal = 0.0f;
        _secondaryRaw = _secondaryMap = _secondaryVal = 0.0f;
        _ethicsRaw = _ethicsMap = _ethicsVal = 0.0f;

        _input.ResetInput();

        UpdateUI();
    }

    private void RestartWorkshop()
    {
        SceneManager.LoadScene(0);
    }

    private void OnInputChanged(InputChangedArgs args)
    {
        _directRaw = CalculateRaw(DirectOptions, args);
        _futhureRaw = CalculateRaw(FuthureOptions, args);
        _secondaryRaw = CalculateRaw(SecondaryOptions, args);
        _ethicsRaw = CalculateEthicRaw(args);

        _directMap = DirectEasing.Evaluate(Mathf.InverseLerp(0.0f, 4.0f, _directRaw));
        _futhureMap = FuthureEasing.Evaluate(Mathf.InverseLerp(0.0f, 4.0f, _futhureRaw));
        _secondaryMap = SecondaryEasing.Evaluate(Mathf.InverseLerp(0.0f, 4.0f, _secondaryRaw));
        _ethicsMap = EthicsEasing.Evaluate(Mathf.InverseLerp(0.0f, 4.0f, _ethicsRaw));

        _directVal = _startBudget * DIREKT_START_MULTI * Mathf.Pow(_directMap, 1.0f / 3.0f);
        _futhureVal = Mathf.Lerp(0.5f, 2.0f, _futhureMap);
        _secondaryVal = _secondaryMap;
        _ethicsVal = _ethicsMap;

        _currentBudget = _startBudget * (1.0f - args.valueSumLevel);
        _nextBudget = _currentBudget + _directVal * _prevFuthureVal;

        UpdateUI();
    }

    private void UpdateUI()
    {
        _directRawField.SetText(_directRaw.ToString("0.000"));
        _directMapField.SetText(_directMap.ToString("0.000"));
        _directValField.SetText(_directVal.ToString("0.0€"));

        _futhureRawField.SetText(_futhureRaw.ToString("0.000"));
        _futhureMapField.SetText(_futhureMap.ToString("0.000"));
        _futhureValField.SetText(_futhureVal.ToString("0.000"));

        _secondaryRawField.SetText(_secondaryRaw.ToString("0.000"));
        _secondaryMapField.SetText(_secondaryMap.ToString("0.000"));
        _secondaryValField.SetText(_secondaryVal.ToString("0.0%"));

        _ethicsRawField.SetText(_ethicsRaw.ToString("0.000"));
        _ethicsMapField.SetText(_ethicsMap.ToString("0.000"));
        _ethicsValField.SetText(_ethicsVal.ToString("0.00"));

        _budget.SetText(_currentBudget.ToString("0.0€"));
        _return.SetText(_nextBudget.ToString("0.0€"));
    }

    private float CalculateRaw(EvaluationOption[] options, InputChangedArgs args)
    {
        return options[0].Evaluate(args.inputOneValue)
            + options[1].Evaluate(args.inputTwoValue)
            + options[2].Evaluate(args.inputThreeValue)
            + options[3].Evaluate(args.inputFourValue);
    }

    private float CalculateEthicRaw(InputChangedArgs args)
    {
        return _ethicsOptions[0].Evaluate(args.inputOneValue)
            - _ethicsOptions[1].Evaluate(args.inputTwoValue)
            + _ethicsOptions[2].Evaluate(args.inputThreeValue)
            - _ethicsOptions[3].Evaluate(args.inputFourValue);
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
