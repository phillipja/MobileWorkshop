using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Graph;
using UnityEngine.SceneManagement;

public class WorkshopManager : MonoBehaviour
{
    const int START_SIZE = 4;

    [Header("Settings")]
    public Settings settings;

    private bool _settingsUpdated;

    [Header("Persons")]
    [SerializeField] private SO_Person[] _persons;

    private SO_Person _selectedPerson;

    [Header("Graph")]
    [SerializeField] private SO_Graph _graph;
    [SerializeField] private SO_GraphCategory _inputCategory;
    [SerializeField] private SO_GraphCategory _directUseCategory;
    [SerializeField] private SO_GraphCategory _outputCategory;

    private Dictionary<SO_GraphNode, float> _startNodes;
    private HashSet<SO_GraphNode> _subGraphNodes;

    [Header("Start Values")]
    [SerializeField, Range(10f, 20f)] private float _startBudget = 10f;
    [SerializeField] private float[] _inputFactors = new float[]
    {
        0.5f, 0.75f, 1.15f, 0.66f
    };

    private float _currentBudget;

    [Header("UI")]
    [SerializeField] private TreasuryMenu _treasuryMenu;
    [SerializeField] private SideEffectsMenu _sideEffectsMenu;
    [SerializeField] private InputMenu _inputMenu;
    [SerializeField] private BenefitsMenu _benefitsMenu;

    public float StartBudget => _startBudget;
    public float CurrentBudget => _currentBudget;
    public float NextBudget => _startBudget * 1.125f * _nextBudgetMulti;
    private float _nextBudgetMulti;

    public event Action OnGraphUpdated;

    private void Start()
    {
        _startNodes = new(START_SIZE);
        _subGraphNodes = new(_graph.nodes.Count);

        GenerateSubgraph();
        PickPerson();

        _treasuryMenu.Init(this);
        _sideEffectsMenu.Init(this, _subGraphNodes);
        _inputMenu.Init(this, _startNodes.Keys.ToArray());
        _benefitsMenu.Init(this, _selectedPerson);
    }

    private void LateUpdate()
    {
        if(_settingsUpdated)
        {
            UpdateInputValues(_inputMenu.CurrentInputValues);
            _settingsUpdated = false;
        }
    }

    #region UI Functions
    /// <summary>
    /// Used in UI Toggle;
    /// </summary>
    /// <param name="active"></param>
    public void UseNormalizedValues(bool active)
    {
        settings.useNormalizedValue = active;
        _settingsUpdated = true;
    }

    /// <summary>
    /// Used in UI Toggle; 
    /// </summary>
    /// <param name="active"></param>
    public void UseEaseFunctions(bool active)
    {
        settings.useEaseFunctions = active;
        _settingsUpdated = true;
    }

    /// <summary>
    /// Used in UI Button;
    /// </summary>
    public void RestartWorkshop()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion

    public void UpdateInputValues(Dictionary<SO_GraphNode, float> currentInputValues)
    {
        foreach(var node in _subGraphNodes)
        {
            node.ClearBufferValue();
        }

        foreach(var kvp in currentInputValues)
        {
            if(_startNodes.ContainsKey(kvp.Key))
            {
                //float value = settings.useNormalizedValue ? kvp.Value
                //    :_startBudget * _startNodes[kvp.Key] * kvp.Value;

                kvp.Key.SetBufferValue(kvp.Value);
            }
        }

        _currentBudget = _startBudget * (1f - _inputMenu.CurrentInputValueSum);
        foreach(var node in _subGraphNodes.Where(n => n.category != _inputCategory))
        {
            node.GetValue(settings);
        }

        int amount =0;
        _nextBudgetMulti = 0f;
        foreach(var node in _subGraphNodes.Where(n => n.category == _directUseCategory))
        {
            _nextBudgetMulti += node.GetValue(settings);
            amount++;
        }
        _nextBudgetMulti /= amount;

        OnGraphUpdated?.Invoke();
    }

    private void PickPerson()
    {
        var availabeOutputs = _subGraphNodes.Where(n => n.category == _outputCategory);

        if(availabeOutputs.Count() == 0)
            return;

        var viablePersons = _persons.Where(p => p.nodes.All(n => availabeOutputs.Contains(n)));
        int amount = viablePersons.Count();

        if(amount == 0)
            return;

        _selectedPerson = viablePersons.ElementAt(UnityEngine.Random.Range(0, amount));
    }

    #region Subgraph Generation
    private void GenerateSubgraph()
    {
        Debug.Log("Generate Subgraph");

        var startNodes = _graph.nodes.Where(n => n.category == _inputCategory).ToArray();
        for(int i = 0; i < START_SIZE; i++)
        {
            bool searchingNode = true;
            while(searchingNode)
            {
                int randIdx = UnityEngine.Random.Range(0, startNodes.Length);
                searchingNode = _startNodes.TryAdd(startNodes[randIdx], _inputFactors[i]) == false;
            }
        }

        foreach(SO_GraphNode node in _startNodes.Keys)
        {
            GetSubgraphRecursive(node, _subGraphNodes);
            _subGraphNodes.Add(node);
        }

        Debug.Log($"Subgraph with {_subGraphNodes.Count} nodes.");
    }

    private void GetSubgraphRecursive(SO_GraphNode node, HashSet<SO_GraphNode> nodes)
    {
        if(node == null || nodes == null)
            return;

        foreach(SO_GraphNode edgeNode in node.toEdges)
        {
            var edge = edgeNode.fromEdges.FirstOrDefault(e => e.node == node);
            if(edge != null)
                edge.IsActive = true;

            GetSubgraphRecursive(edgeNode, nodes);
            nodes.Add(edgeNode);
        }
    }
    #endregion
}

[Serializable]
public struct Settings
{
    public bool useNormalizedValue;
    public bool useEaseFunctions;
}
