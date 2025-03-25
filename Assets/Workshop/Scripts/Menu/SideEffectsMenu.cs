using Graph;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SideEffectsMenu : MonoBehaviour
{
    [SerializeField] private LevelIndicator _futhureField;
    [SerializeField] private SO_GraphCategory _futhureCategory;

    [SerializeField] private LevelIndicator _secondaryField;
    [SerializeField] private SO_GraphCategory _secondaryCategory;

    [SerializeField] private LevelIndicator _ethicsField;
    [SerializeField] private SO_GraphCategory _ethicsCategory;

    private SO_GraphNode[] _futhureNodes;
    private SO_GraphNode[] _secondaryNodes;
    private SO_GraphNode[] _ethicsNodes;

    private WorkshopManager _manager;

    public void Init(WorkshopManager manager, HashSet<SO_GraphNode> subGraph)
    {
        _manager = manager;
        _manager.OnGraphUpdated += OnGraphUpdated;

        _futhureField.SetValue(0f);
        _futhureNodes = subGraph.Where(n => n.category == _futhureCategory).ToArray();

        _secondaryField.SetValue(0f);
        _secondaryNodes = subGraph.Where(n => n.category == _secondaryCategory).ToArray();

        _ethicsField.SetValue(0f);
        _ethicsNodes = subGraph.Where(n => n.category == _ethicsCategory).ToArray();
    }

    private void OnDestroy()
    {
        _manager.OnGraphUpdated -= OnGraphUpdated;
    }

    private void OnGraphUpdated()
    {
        UpdateUI(_futhureNodes, _futhureField);
        UpdateUI(_secondaryNodes, _secondaryField);
        UpdateUI(_ethicsNodes, _ethicsField);
    }

    private void UpdateUI(SO_GraphNode[] nodes, LevelIndicator indicator)
    {
        var sum = 0f;
        foreach(var node in nodes)
        {
            sum += node.GetValue();
        }
        indicator.SetValue(sum);
    }
}
