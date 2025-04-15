using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewCalculation : MonoBehaviour
{
    const float PHASE = 3 * Mathf.PI / 2;

    [SerializeField] private Slider _inputA;
    [SerializeField] private Slider _inputB;
    [SerializeField] private Slider _inputC;
    [SerializeField] private Slider _inputD;
    [Space]
    [SerializeField] private Vector4 _connectionsA;
    [SerializeField] private Vector4 _connectionsB;
    [SerializeField] private Vector4 _connectionsC;
    [SerializeField] private Vector4 _connectionsD;
    [Space]
    [SerializeField] private TextMeshProUGUI _outputA;
    [SerializeField] private TextMeshProUGUI _outputB;
    [SerializeField] private TextMeshProUGUI _outputC;
    [SerializeField] private TextMeshProUGUI _outputD;

    private float _valueA;
    private float _valueB;
    private float _valueC;
    private float _valueD;

    private void Update()
    {
        //_valueA = _inputA.value * _connectionsA.x + _inputB.value * _connectionsB.x
        //    + _inputC.value * _connectionsC.x + _inputD.value * _connectionsD.x;

        //_valueB = _inputA.value * _connectionsA.y + _inputB.value * _connectionsB.y
        //    + _inputC.value * _connectionsC.y + _inputD.value * _connectionsD.y;

        //_valueC = _inputA.value * _connectionsA.z + _inputB.value * _connectionsB.z
        //    + _inputC.value * _connectionsC.z + _inputD.value * _connectionsD.z;

        //_valueD = _inputA.value * _connectionsA.w + _inputB.value * _connectionsB.w
        //    + _inputC.value * _connectionsC.w + _inputD.value * _connectionsD.w;

        CalculateA();
        CalculateB();
        CalculateC();
        CalculateD();

        _outputA.text = _valueA.ToString("0.000");
        _outputB.text = _valueB.ToString("0.000");
        _outputC.text = _valueC.ToString("0.000");
        _outputD.text = _valueD.ToString("0.000");
    }

    private void CalculateA()
    {
        _valueA = TrippleSin(_inputA.value, 5, 1, 7) * _connectionsA.x + DoubleSin(_inputB.value, 7, 3) * _connectionsB.x
            + DoubleSin(_inputC.value, 3, 5) * _connectionsC.x + TrippleSin(_inputD.value, 1, 7, 3) * _connectionsD.x;
    }

    private void CalculateB()
    {
        _valueB = DoubleSin(_inputA.value, 5, 7) * _connectionsA.y + TrippleSin(_inputB.value, 3, 7, 9) * _connectionsB.y
            + TrippleSin(_inputC.value, 1, 5, 3) * _connectionsC.y + DoubleSin(_inputD.value, 1, 3) * _connectionsD.y;
    }

    private void CalculateC()
    {
        _valueC = TrippleSin(_inputA.value, 5, 7, 1) * _connectionsA.z + DoubleSin(_inputB.value, 5, 3) * _connectionsB.z
            + TrippleSin(_inputC.value, 1, 3, 5) * _connectionsC.z + DoubleSin(_inputD.value, 1, 5) * _connectionsD.z;
    }

    private void CalculateD()
    {
        _valueD = DoubleSin(_inputA.value, 3, 1) * _connectionsA.w + TrippleSin(_inputB.value, 5, 7, 3) * _connectionsB.w
            + DoubleSin(_inputC.value, 3, 9) * _connectionsC.w + TrippleSin(_inputD.value, 3, 11, 9) * _connectionsD.w;
    }

    private float DoubleSin(float x, int a, int b)
    {
        return 1 + ((Mathf.Sin(PHASE + a * x * Mathf.PI) + Mathf.Sin(PHASE + b * x * Mathf.PI)) / 4);
    }

    private float TrippleSin(float x, int a, int b, int c)
    {
        return 1 + ((Mathf.Sin(PHASE + a * x * Mathf.PI) + Mathf.Sin(PHASE + b * x * Mathf.PI) + Mathf.Sin(PHASE + c * x * Mathf.PI)) / 6);
    }
}
