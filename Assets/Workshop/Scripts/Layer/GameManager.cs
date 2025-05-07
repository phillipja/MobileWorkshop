using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private const float DIREKT_START_MULTI = 1.25992104989f;

    [SerializeField] private EvaluationLayer _layer;
    [Header("Round")]
    [SerializeField] private TextMeshProUGUI _roundLable;
    [SerializeField] private TextMeshProUGUI _budget;
    [SerializeField] private TextMeshProUGUI _return;
    [SerializeField] private Button _endRoundButton;
    [SerializeField] private GameObject _endScreen;
    [SerializeField] private TextMeshProUGUI _outro;
    [SerializeField] private GameObject _endPanel;
    [SerializeField] private Button _continue;
    [SerializeField] private Button _restart;

    private float _startBudget;
    private float _currentBudget;
    private float _nextBudget;
    private float _prevFuthureVal;
    private float _prevSecondaryVal;
    private int _round;

    [Header("Output")]
    [SerializeField] private TextMeshProUGUI _directValField;
    [SerializeField] private TextMeshProUGUI _futhureValField;
    [SerializeField] private TextMeshProUGUI _secondaryValField;
    [SerializeField] private TextMeshProUGUI _ethicsValField;

    private float _directVal;
    private float _futhureVal;
    private float _secondaryVal;
    private float _ethicsVal;

    [field: SerializeField, Range(0f, 1f)] public float DirectBreakpoint { get; set; } = 0.5f;
    [field: SerializeField, Range(1f, 4f)] public float FuthureMulti { get; set; } = 2.0f;
    [field: SerializeField, Range(0f, 1f)] public float SecondaryContinue { get; set; } = 0.5f;
    [field: SerializeField, Range(0.5f, 2f)] public float EthicsMulti { get; set; } = 1.0f;

    private void Start()
    {
        _startBudget = _currentBudget = 15.0f;
        _nextBudget = 0.0f;
        _prevFuthureVal = 1.0f;
        _round = 1;

        _layer.OnEvaluationChanged += OnEvaluationChanged;

        _endRoundButton.onClick.AddListener(EndRound);
        _continue.onClick.AddListener(Continue);
        _restart.onClick.AddListener(RestartWorkshop);

        PrepareRound();
    }

    private void OnDestroy()
    {
        _layer.OnEvaluationChanged -= OnEvaluationChanged;

        _endRoundButton.onClick.RemoveListener(EndRound);
        _continue.onClick.RemoveListener(Continue);
        _restart.onClick.RemoveListener(RestartWorkshop);
    }

    private void EndRound()
    {
        var r = Random.Range(0f, 1f);

        if (r <= _secondaryVal)
        {
            _prevSecondaryVal = 0.0f;
            var bonus = (10 * _round * _round) * _ethicsVal;
            _nextBudget += bonus;

            if (bonus > 0)
            {
                _outro.SetText($"You have been rewarded with {bonus:0.0€} for your ethical decisions." +
                    $"\n\nYour final balance for this round is {_nextBudget:0.0€}.");
            }
            else
            {
                if (_nextBudget > 0)
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
                    _endPanel.SetActive(true);
                }
            }
        }
        else
        {
            _prevSecondaryVal = _secondaryVal * SecondaryContinue;
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

        if (_round > 2)
        {
            var r = Random.Range(0f, 1f);
            float probability = 1.0f / ((float)_round - 2.0f);
            bool continueGame = r < probability;
            _continue.gameObject.SetActive(continueGame);
            _restart.gameObject.SetActive(!continueGame);
            _endPanel.SetActive(!continueGame);
        }

        PrepareRound();
    }

    private void PrepareRound()
    {
        _directVal = 0.0f;
        _futhureVal = 0.0f;
        _secondaryVal = _prevSecondaryVal;
        _ethicsVal = 0.0f;
        _currentBudget = _startBudget;

        _layer.ResetEvaluation();

        UpdateUI();
    }

    private void RestartWorkshop()
    {
        SceneManager.LoadScene(0);
    }

    private void OnEvaluationChanged(EvaluationChangedArgs args)
    {
        _directVal = _startBudget * (1.0f / Mathf.Pow(DirectBreakpoint, 1.0f / 3.0f)) * Mathf.Pow(args.directMap, 1.0f / 3.0f);
        _futhureVal = Mathf.Lerp(1.0f / FuthureMulti, FuthureMulti, args.futhureMap);
        _secondaryVal = _prevSecondaryVal + args.secondaryMap;
        _ethicsVal = EthicsMulti * args.ethicsMap;

        _currentBudget = _startBudget * args.budgetMultiplier;
        _currentBudget *= _currentBudget < 0 ? 2.0f : 1.0f;
        _nextBudget = _currentBudget + _directVal * _prevFuthureVal;

        UpdateUI();
    }

    private void UpdateUI()
    {
        _directValField.SetText(_directVal.ToString("0.0€"));
        _futhureValField.SetText(_futhureVal.ToString("0.00"));
        _secondaryValField.SetText(_secondaryVal.ToString("0.0%"));
        _ethicsValField.SetText(_ethicsVal.ToString("0.00"));

        _budget.SetText(_currentBudget.ToString("0.0€"));
        _return.SetText(_nextBudget.ToString("0.0€"));
    }
}
