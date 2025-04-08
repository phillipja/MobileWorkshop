using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TreasuryMenu : MonoBehaviour
{
    [SerializeField] private LevelIndicator _budgetField;
    [SerializeField] private TextMeshProUGUI _profitValueField;

    private WorkshopManager _manager;

    public void Init(WorkshopManager manager)
    {
        _manager = manager;

        _manager.OnGraphUpdated += OnGraphUpdated;
        _budgetField.maxValue = _manager.StartBudget;
        SetBudgetValues(_manager.StartBudget, _manager.NextBudget);
    }

    private void OnGraphUpdated()
    {
        SetBudgetValues(_manager.CurrentBudget, _manager.NextBudget);
    }

    private void SetBudgetValues(float currentBudget, float nextBudget)
    {
        _budgetField.SetValue(currentBudget, "0.0");
        _profitValueField.text = nextBudget.ToString("0.0");
    }

    private void OnDestroy()
    {
        _manager.OnGraphUpdated -= OnGraphUpdated;
    }
}