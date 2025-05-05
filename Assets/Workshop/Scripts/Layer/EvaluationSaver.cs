using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class EvaluationSaver : MonoBehaviour
{
    [SerializeField] private EvaluationLayer _evaluationLayer;
    [SerializeField] private bool _loadOnStart = true;
    [SerializeField] private string _defaultFileName = "evaluation_data.json";

    public string JsonFilePath => Path.Combine(Application.persistentDataPath, _defaultFileName);

    [Serializable]
    public class EvaluationData
    {
        public CategoryData Direct;
        public CategoryData Future;
        public CategoryData Secondary;
        public CategoryData Ethics;
    }

    [Serializable]
    public class CategoryData
    {
        public OptionData[] Options;
        public int EasingType;
    }

    [Serializable]
    public class OptionData
    {
        public string Type;
        public float Weight;
        public float[] Variables;
    }

    private void Start()
    {
        if (_loadOnStart && File.Exists(JsonFilePath))
        {
            LoadFromJson();
        }
    }

    public void SaveToJson()
    {
        if (_evaluationLayer == null)
        {
            Debug.LogError("EvaluationLayer not assigned!");
            return;
        }

        EvaluationData data = new EvaluationData
        {
            Direct = ExtractCategoryData(_evaluationLayer.DirectOptions, (int)_evaluationLayer.DirectEasing),
            Future = ExtractCategoryData(_evaluationLayer.FuthureOptions, (int)_evaluationLayer.FuthureEasing),
            Secondary = ExtractCategoryData(_evaluationLayer.SecondaryOptions, (int)_evaluationLayer.SecondaryEasing),
            Ethics = ExtractCategoryData(_evaluationLayer.EthicsOptions, (int)_evaluationLayer.EthicsEasing)
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(JsonFilePath, json);
        Debug.Log($"Evaluation data saved to: {JsonFilePath}");
    }

    public void SaveEvaluationData()
    {
        SaveToJson();
    }

    private CategoryData ExtractCategoryData(EvaluationOption[] options, int easingType)
    {
        CategoryData category = new CategoryData
        {
            Options = new OptionData[options.Length],
            EasingType = easingType
        };

        for (int i = 0; i < options.Length; i++)
        {
            EvaluationOption option = options[i];
            List<float> variables = new List<float>();

            foreach (var floatRange in option.GetVariables())
            {
                variables.Add(floatRange.value);
            }

            category.Options[i] = new OptionData
            {
                Type = option.GetEvaluationType().ToString(),
                Weight = option.IsPositive ? 1.0f : -1.0f,
                Variables = variables.ToArray()
            };
        }

        return category;
    }

    public void LoadFromJson()
    {
        LoadFromJson(JsonFilePath);
    }

    public void LoadFromJson(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError($"File not found: {path}");
            return;
        }

        try
        {
            string json = File.ReadAllText(path);
            EvaluationData data = JsonUtility.FromJson<EvaluationData>(json);

            ApplyEvaluationData(data);
            Debug.Log($"Evaluation data loaded from: {path}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading evaluation data: {e.Message}");
        }
    }

    private void ApplyEvaluationData(EvaluationData data)
    {
        if (_evaluationLayer == null || data == null)
            return;

        _evaluationLayer.DirectEasing = (EasingType)data.Direct.EasingType;
        _evaluationLayer.FuthureEasing = (EasingType)data.Future.EasingType;
        _evaluationLayer.SecondaryEasing = (EasingType)data.Secondary.EasingType;
        _evaluationLayer.EthicsEasing = (EasingType)data.Ethics.EasingType;

        ApplyOptionData(_evaluationLayer.DirectOptions, data.Direct.Options);
        ApplyOptionData(_evaluationLayer.FuthureOptions, data.Future.Options);
        ApplyOptionData(_evaluationLayer.SecondaryOptions, data.Secondary.Options);
        ApplyOptionData(_evaluationLayer.EthicsOptions, data.Ethics.Options);
    }

    private void ApplyOptionData(EvaluationOption[] options, OptionData[] optionData)
    {
        if (options.Length != optionData.Length)
        {
            Debug.LogError($"Option count mismatch: {options.Length} vs {optionData.Length}");
            return;
        }

        for (int i = 0; i < options.Length; i++)
        {
            string currentType = options[i].GetEvaluationType().ToString();
            if (currentType != optionData[i].Type)
            {
                if (Enum.TryParse<EvaluationType>(optionData[i].Type, out EvaluationType type))
                {
                    options[i] = EvaluationOption.CreateInstance(type);
                }
                else
                {
                    Debug.LogError($"Unknown evaluation type: {optionData[i].Type}");
                    continue;
                }
            }

            if ((optionData[i].Weight < 0 && options[i].IsPositive) ||
                (optionData[i].Weight > 0 && !options[i].IsPositive))
            {
                options[i].ChangeWeight();
            }

            options[i].SetVariables(optionData[i].Variables);
        }
    }
}