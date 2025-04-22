using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewCalculation : MonoBehaviour
{
    const float PHASE = 3 * Mathf.PI / 2;
    const float DIREKT_START_MULTI = 1.25992104989f;

    [SerializeField] private GameObject _roundOneIntro;
    [SerializeField] private GameObject _roundTwoIntro;
    [SerializeField] private GameObject _roundThreeIntro;
    [Space]
    [SerializeField] private TextMeshProUGUI _budgetIn;
    [SerializeField] private TextMeshProUGUI _budgetOut;
    [Space]
    [SerializeField] private Slider _inputA;
    [SerializeField] private Slider _inputB;
    [SerializeField] private Slider _inputC;
    [SerializeField] private Slider _inputD;
    [Space]
    [SerializeField] private TextMeshProUGUI _outputA;
    [SerializeField] private TextMeshProUGUI _outputB;
    [SerializeField] private TextMeshProUGUI _outputC;
    [SerializeField] private TextMeshProUGUI _outputD;
    [Space]
    [SerializeField] private TextMeshProUGUI _roundLable;
    [SerializeField] private Button _endRound;
    [SerializeField] private GameObject _endScreen;
    [SerializeField] private TextMeshProUGUI _outro;
    [SerializeField] private Button _continue;
    [SerializeField] private Button _restart;

    private int[] _inputAFreq;
    private int[] _inputBFreq;
    private int[] _inputCFreq;
    private int[] _inputDFreq;

    private float _valueA;
    private float _valueB;
    private float _valueC;
    private float _valueD;

    private float _valSum;

    private float _valueA01;
    private float _valueB01;
    private float _valueC01;
    private float _valueD01;

    private float _startBudget = 15f;

    private float _direct;
    private float _futhure;
    private float _secondary;
    private float _ethics;

    private float _lastFuture = 1f;

    private float _nextBudget;

    private int _round;

    private void Start()
    {
        _inputA.onValueChanged.AddListener(InputAChanged);
        _inputB.onValueChanged.AddListener(InputBChanged);
        _inputC.onValueChanged.AddListener(InputCChanged);
        _inputD.onValueChanged.AddListener(InputDChanged);

        _endRound.onClick.AddListener(EndRound);
        _continue.onClick.AddListener(Continue);
        _restart.onClick.AddListener(RestartWorkshop);

        Continue();
    }

    private void RestartWorkshop()
    {
        SceneManager.LoadScene(0);
    }

    private void Continue()
    {
        _round++;
        _roundLable.SetText(_round.ToString());

        if(_round > 2)
        {
            _continue.gameObject.SetActive(false);
            _restart.gameObject.SetActive(true);
        }

        PrepareRound();
    }

    private void EndRound()
    {
        var r = Random.Range(0f, 1f);

        if(r <= _secondary)
        {
            var bonus = (10 * _round * _round) * _ethics;
            _nextBudget += bonus;

            if(bonus > 0)
            {
                _outro.SetText($"You have been rewarded with {bonus:0.0} for your ethical decisions." +
                    $"\n\nYour final balance for this round is {_nextBudget:0.0}.");
            }
            else
            {
                if(_nextBudget > 0)
                {
                    _outro.SetText($"You have been punished with {bonus:0.0} for your unethical decisions." +
                        $"\n\nYour final balance for this round is {_nextBudget:0.0}.");
                }
                else
                {
                    _outro.SetText($"You have been punished with {bonus:0.0} for your unethical decisions." +
                        $"\n\nYou bankrupted the company by your decisions. Final balance: {_nextBudget:0.0}.");

                    _continue.gameObject.SetActive(false);
                    _restart.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            _outro.SetText($"Your decisions have been too insignificant. The public has not become aware of you." +
                    $"\n\nYour final return for this round is {_nextBudget:0.0}.");
        }

        _startBudget = _nextBudget;
        _lastFuture = _futhure;

        _endScreen.SetActive(true);
    }

    private void PrepareRound()
    {
        switch(_round)
        {
            case 1:
                _roundOneIntro.SetActive(true);
                break;
            case 2:
                _roundTwoIntro.SetActive(true);
                break;
            case 3:
                _roundThreeIntro.SetActive(true);
                break;

            default:
                Debug.LogError("Invalid round number: " + _round);
                return;
        }

        _direct = 0f;
        _futhure = 0f;
        _secondary = 0f;
        _ethics = 0f;

        _valueA = 0f;
        _valueB = 0f;
        _valueC = 0f;
        _valueD = 0f;

        _valSum = 0f;

        _inputA.SetValueWithoutNotify(_valueA);
        _inputB.SetValueWithoutNotify(_valueB);
        _inputC.SetValueWithoutNotify(_valueC);
        _inputD.SetValueWithoutNotify(_valueD);

        _inputAFreq = GenerateFreq();
        _inputBFreq = GenerateFreq();
        _inputCFreq = GenerateFreq();
        _inputDFreq = GenerateFreq();

        UpdateOutputs();
    }

    private int[] GenerateFreq()
    {
        var inputFreq = new int[10];
        for(int i = 0; i < inputFreq.Length; i++)
        {
            inputFreq[i] = Random.Range(1, 17);
        }
        return inputFreq;
    }

    private void OnDestroy()
    {
        _inputA.onValueChanged.RemoveListener(InputAChanged);
        _inputB.onValueChanged.RemoveListener(InputBChanged);
        _inputC.onValueChanged.RemoveListener(InputCChanged);
        _inputD.onValueChanged.RemoveListener(InputDChanged);

        _endRound.onClick.RemoveListener(EndRound);
        _continue.onClick.RemoveListener(Continue);
        _restart.onClick.RemoveListener(RestartWorkshop);
    }

    private void InputAChanged(float newVal)
    {
        _valueA = EnsureValue(_valueA, newVal);
        _inputA.SetValueWithoutNotify(_valueA);
        UpdateOutputs();
    }

    private void InputBChanged(float newVal)
    {
        _valueB = EnsureValue(_valueB, newVal);
        _inputB.SetValueWithoutNotify(_valueB);
        UpdateOutputs();
    }

    private void InputCChanged(float newVal)
    {
        _valueC = EnsureValue(_valueC, newVal);
        _inputC.SetValueWithoutNotify(_valueC);
        UpdateOutputs();
    }

    private void InputDChanged(float newVal)
    {
        _valueD = EnsureValue(_valueD, newVal);
        _inputD.SetValueWithoutNotify(_valueD);
        UpdateOutputs();
    }

    private float EnsureValue(float oldVal, float newVal)
    {
        _valSum -= oldVal;

        if(newVal < oldVal)
        {
            _valSum += newVal;
            return newVal;
        }
        else
        {
            float ensuredVal = Mathf.Min(newVal, 40 - _valSum);
            _valSum += ensuredVal;
            return ensuredVal;
        }
    }

    private void UpdateOutputs()
    {
        _valueA01 = ValueTo01(_valueA);
        _valueB01 = ValueTo01(_valueB);
        _valueC01 = ValueTo01(_valueC);
        _valueD01 = ValueTo01(_valueD);

        var a = CalculateA();
        var b = CalculateB();
        var c = CalculateC();
        var d = CalculateD();

        _direct = _startBudget * DIREKT_START_MULTI * Mathf.Lerp(0f, 1f, Mathf.InverseLerp(0f, 4f, a));
        _futhure = Mathf.Lerp(0.5f, 1.5f, Mathf.InverseLerp(0f, 4f, b));
        _secondary = Mathf.Lerp(0f, 1f, Mathf.InverseLerp(0f, 4f, c));
        _ethics = Mathf.Lerp(-1f, 1f, Mathf.InverseLerp(0f, 4f, d));

        _outputA.SetText(_direct.ToString("0.00"));
        _outputB.SetText(_futhure.ToString("0.00"));
        _outputC.SetText(_secondary.ToString("0.00"));
        _outputD.SetText(_ethics.ToString("0.00"));

        UpdateBudget();
    }

    private void UpdateBudget()
    {
        var current = Mathf.Lerp(_startBudget, 0f, Mathf.InverseLerp(0f, 40f, _valSum));
        _budgetIn.SetText(current.ToString("0.0"));

        _nextBudget = current + _direct * _lastFuture;
        _budgetOut.SetText(_nextBudget.ToString("0.0"));
    }

    private float ValueTo01(float value)
    {
        return Mathf.InverseLerp(0, 20, value);
    }

    private float CalculateA()
    {
        return TrippleSin(_valueA01, _inputAFreq[0], _inputAFreq[1], _inputAFreq[2])
            + DoubleSin(_valueB01, _inputBFreq[0], _inputBFreq[1])
            + DoubleSin(_valueC01, _inputCFreq[0], _inputCFreq[1])
            + TrippleSin(_valueD01, _inputDFreq[0], _inputDFreq[1], _inputDFreq[2]);
    }

    private float CalculateB()
    {
        return DoubleSin(_valueA01, _inputAFreq[3], _inputAFreq[4])
            + TrippleSin(_valueB01, _inputBFreq[2], _inputBFreq[3], _inputBFreq[4])
            + TrippleSin(_valueC01, _inputCFreq[2], _inputCFreq[3], _inputCFreq[4])
            + DoubleSin(_valueD01, _inputDFreq[3], _inputDFreq[4]);
    }

    private float CalculateC()
    {
        return TrippleSin(_valueA01, _inputAFreq[5], _inputAFreq[6], _inputAFreq[7])
            + DoubleSin(_valueB01, _inputBFreq[5], _inputBFreq[6])
            + TrippleSin(_valueC01, _inputCFreq[5], _inputCFreq[6], _inputCFreq[7])
            + DoubleSin(_valueD01, _inputDFreq[5], _inputDFreq[6]);
    }

    private float CalculateD()
    {
        return DoubleSin(_valueA01, _inputAFreq[8], _inputAFreq[9])
            + TrippleSin(_valueB01, _inputBFreq[7], _inputBFreq[8], _inputBFreq[9])
            + DoubleSin(_valueC01, _inputCFreq[8], _inputCFreq[9])
            + TrippleSin(_valueD01, _inputDFreq[7], _inputDFreq[8], _inputDFreq[9]);
    }

    private float DoubleSin(float x, int a, int b)
    {
        return 0.5f + ((Mathf.Sin(PHASE + a * x * Mathf.PI) + Mathf.Sin(PHASE + b * x * Mathf.PI)) / 4);
    }

    private float TrippleSin(float x, int a, int b, int c)
    {
        return 0.5f + ((Mathf.Sin(PHASE + a * x * Mathf.PI) + Mathf.Sin(PHASE + b * x * Mathf.PI) + Mathf.Sin(PHASE + c * x * Mathf.PI)) / 6);
    }
}
