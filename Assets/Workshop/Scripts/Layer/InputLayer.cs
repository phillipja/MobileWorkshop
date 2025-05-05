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
        foreach (var input in GetSlider()) 
        {
            input.valueChanged = EnsureValue;
        }
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
        foreach (var input in GetSlider())
        {
            input.valueChanged = null;
        }
    }

    private float EnsureValue(float oldVal, float newVal)
    {
        _valueSum -= oldVal;
        _valueSum += newVal;

        _inputChanged = oldVal != newVal;
        return newVal;

        //if (newVal < oldVal)
        //{
        //    _valueSum += newVal;
        //    _inputChanged = true;
        //    return newVal;
        //}
        //else
        //{
        //    float ensuredVal = Mathf.Min(newVal, _maxValueSum - _valueSum);
        //    _valueSum += ensuredVal;
        //    _inputChanged = oldVal - ensuredVal != 0f;
        //    return ensuredVal;
        //}
    }

    public void ResetInput()
    {
        foreach (var input in GetSlider())
        {
            input.ResetValue();
        }
        _valueSum = 0f;
    }

    private IEnumerable<InputSlider> GetSlider()
    {
        yield return _inputOne;
        yield return _inputTwo;
        yield return _inputThree;
        yield return _inputFour;
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
