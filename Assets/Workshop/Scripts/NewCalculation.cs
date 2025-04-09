using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewCalculation : MonoBehaviour
{
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
        _valueA = _inputA.value * _connectionsA.x + _inputB.value * _connectionsB.x
            + _inputC.value * _connectionsC.x + _inputD.value * _connectionsD.x;

        _valueB = _inputA.value * _connectionsA.y + _inputB.value * _connectionsB.y
            + _inputC.value * _connectionsC.y + _inputD.value * _connectionsD.y;

        _valueC = _inputA.value * _connectionsA.z + _inputB.value * _connectionsB.z
            + _inputC.value * _connectionsC.z + _inputD.value * _connectionsD.z;

        _valueD = _inputA.value * _connectionsA.w + _inputB.value * _connectionsB.w
            + _inputC.value * _connectionsC.w + _inputD.value * _connectionsD.w;

        _outputA.text = _valueA.ToString("0.000");
        _outputB.text = _valueB.ToString("0.000");
        _outputC.text = _valueC.ToString("0.000");
        _outputD.text = _valueD.ToString("0.000");
    }
}
