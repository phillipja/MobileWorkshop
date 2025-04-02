using Graph;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BenefitsMenu : MonoBehaviour
{
    [SerializeField] private LevelIndicator _benefitOneField;
    [SerializeField] private LevelIndicator _benefitTwoField;
    [SerializeField] private LevelIndicator _benefitThreeField;

    private WorkshopManager _manager;
    private SO_Person _person;

    public void Init(WorkshopManager manager, SO_Person person)
    {
        _manager = manager;
        _manager.OnGraphUpdated += OnGraphUpdated;

        _person = person;

        _benefitOneField.SetTitel(_person.nodes[0].description);
        _benefitTwoField.SetTitel(_person.nodes[1].description);
        _benefitThreeField.SetTitel(_person.nodes[2].description);

        _benefitOneField.SetValue(0f);
        _benefitTwoField.SetValue(0f);
        _benefitThreeField.SetValue(0f);
    }


    private void OnDestroy()
    {
        _manager.OnGraphUpdated -= OnGraphUpdated;
    }

    private void OnGraphUpdated()
    {
        UpdateUI(_person.nodes[0], _benefitOneField);
        UpdateUI(_person.nodes[1], _benefitTwoField);
        UpdateUI(_person.nodes[2], _benefitThreeField);
    }

    private void UpdateUI(SO_GraphNode node, LevelIndicator indicator)
    {
        float value = node.GetValue(_manager.settings);
        indicator.SetValue(value);
    }
}
