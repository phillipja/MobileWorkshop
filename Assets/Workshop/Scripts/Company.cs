using Graph;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class Company : MonoBehaviour
{
    const int START_SIZE = 4;
    [SerializeField] private List<SO_Person> _persons;
    [Space]
    [SerializeField] private SO_Graph _graph;
    [SerializeField] private SO_GraphCategory _controlVariableCategory;
    [SerializeField] private SO_GraphCategory _personalBenefitCategory;
    [Space]
    [SerializeField] private InputAction _regenerateSubgraph;
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _person;
    [Space]
    [SerializeField] private Transform _startNodesContainer;
    [SerializeField] private Transform _edgesContainer;
    [SerializeField] private Transform _nodesContainer;
    [Space]
    [SerializeField] private Single _singlePrefab;
    [SerializeField] private Double _doublePrefab;
    [Header("Runtime")]
    [SerializeField] private List<SO_Person> _viableCandidates = new(10);


    private HashSet<SO_GraphNode> _startNodes;
    private HashSet<SO_GraphNode> _nodes;

    private List<GameObject> _uiStartNodes;
    private List<GameObject> _uiNodes;

    void Start()
    {
        Assert.IsNotNull(_graph, "Missing Graph!");
        Assert.IsNotNull(_controlVariableCategory, "Missing category for control variables!");
        Assert.IsNotNull(_personalBenefitCategory, "Missing category for personal benefit!");

        _startNodes = new(START_SIZE);
        _nodes = new(_graph.nodes.Count);

        _uiStartNodes = new(START_SIZE);
        _uiNodes = new(_graph.nodes.Count);

        GenerateSubgraph();
        FindPersonalities();
    }

    private void OnEnable()
    {
        _regenerateSubgraph.Enable();
        _regenerateSubgraph.performed += RegenerateSubgraphPerformed;
    }

    private void OnDisable()
    {
        _regenerateSubgraph.performed -= RegenerateSubgraphPerformed;
        _regenerateSubgraph.Disable();
    }

    private void RegenerateSubgraphPerformed(InputAction.CallbackContext args)
    {
        _startNodes.Clear();
        _nodes.Clear();

        ClearUIElements(_uiStartNodes);
        ClearUIElements(_uiNodes);

        GenerateSubgraph();
        FindPersonalities();

        void ClearUIElements(List<GameObject> elements)
        {
            foreach(var element in elements)
            {
                Destroy(element);
            }
            elements.Clear();
        }
    }

    private void GenerateSubgraph()
    {
        Debug.Log("Generate Subgraph");

        var startNodes = _graph.nodes.Where(n => n.category == _controlVariableCategory).ToArray();
        for(int i = 0; i < START_SIZE; i++)
        {
            bool searchingNode = true;
            while(searchingNode)
            {
                int randIdx = UnityEngine.Random.Range(0, startNodes.Length);
                searchingNode = !_startNodes.Add(startNodes[randIdx]);
            }
        }

        foreach(SO_GraphNode node in _startNodes)
        {
            _nodes.Add(node);
            GetSubgraphRecursive(node, _nodes);
        }

        _startNodes = _startNodes.OrderBy(n => n.index).ToHashSet();
        _nodes = _nodes.OrderBy(n => n.id).ToHashSet();

        Debug.Log($"Subgraph size: Nodes '{_nodes.Count}'");

        foreach(var startNode in _startNodes)
        {
            var uiNode = Instantiate(_singlePrefab, _startNodesContainer);
            uiNode.Text.text = startNode.id;
            _uiStartNodes.Add(uiNode.gameObject);
        }

        foreach(var node in _nodes)
        {
            var uiNode = Instantiate(_singlePrefab, _nodesContainer);
            uiNode.Text.text = node.id;
            _uiNodes.Add(uiNode.gameObject);
        }
    }

    private void GetSubgraphRecursive(SO_GraphNode node, HashSet<SO_GraphNode> nodes)
    {
        if(node == null || nodes == null)
            return;

        foreach(MathModeledEdge edge in node.toEdges)
        {
            GetSubgraphRecursive(edge.node, nodes);
            nodes.Add(edge.node);
        }
    }

    private void FindPersonalities()
    {
        _viableCandidates.Clear();
        var persBenefit = _nodes.Where(n => n.category == _personalBenefitCategory).ToList();

        if(persBenefit.Count == 0)
            return;

        foreach(var person in _persons)
        {
            bool isViable = person.nodes.All(n => persBenefit.Contains(n));
            if(isViable)
                _viableCandidates.Add(person);
        }

        _person.text = "";
        foreach(var person in _viableCandidates)
        {
            _person.text += $"{person.forename} {person.surname}, ";
        }
    }
}
