using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputLayer : MonoBehaviour
{
    [SerializeField] private InputSlider _inputOne;
    [SerializeField] private InputSlider _inputTwo;
    [SerializeField] private InputSlider _inputThree;
    [SerializeField] private InputSlider _inputFour;
    [Space]
    [SerializeField, Range(1, 4)] private int _maxValueSum = 1;

    public delegate void InputChanged(InputChangedArgs args);
    public event InputChanged OnInputChanged;

    private bool _inputChanged;

    private float _valueSum;

    void Start()
    {
        _inputOne.valueChanged = EnsureValue;
        _inputTwo.valueChanged = EnsureValue;
        _inputThree.valueChanged = EnsureValue;
        _inputFour.valueChanged = EnsureValue;
    }

    private void LateUpdate()
    {
        if (_inputChanged)
        {
            OnInputChanged?.Invoke(new()
            {
                valueSumLevel = _valueSum / _maxValueSum,
                inputOneValue = _inputOne.Value,
                inputTwoValue = _inputTwo.Value,
                inputThreeValue = _inputThree.Value,
                inputFourValue = _inputFour.Value,
            });

            _inputChanged = false;
        }
    }

    private void OnDestroy()
    {
        _inputOne.valueChanged = null;
        _inputTwo.valueChanged = null;
        _inputThree.valueChanged = null;
        _inputFour.valueChanged = null;
    }

    private float EnsureValue(float oldVal, float newVal)
    {
        _valueSum -= oldVal;

        if (newVal < oldVal)
        {
            _valueSum += newVal;
            _inputChanged = true;
            return newVal;
        }
        else
        {
            float ensuredVal = Mathf.Min(newVal, _maxValueSum - _valueSum);
            _valueSum += ensuredVal;
            _inputChanged = oldVal - ensuredVal != 0f;
            return ensuredVal;
        }
    }

    public void ResetInput()
    {
        _valueSum = 0f;
        _inputOne.SetValue(0);
        _inputTwo.SetValue(0);
        _inputThree.SetValue(0);
        _inputFour.SetValue(0);
    }
}

public struct InputChangedArgs
{
    public float valueSumLevel;
    public float inputOneValue;
    public float inputTwoValue;
    public float inputThreeValue;
    public float inputFourValue;
}
