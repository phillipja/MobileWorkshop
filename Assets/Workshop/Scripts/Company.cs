using Graph;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class Company : MonoBehaviour
{
    const int START_SIZE = 4;

    [SerializeField] private SO_Graph _graph;
    [SerializeField] private SO_GraphCategory _controlVarCategory;
    [Space]
    [SerializeField] private InputAction _regenerateSubgraph;
    [Header("UI")]
    [SerializeField] private Transform _startNodesContainer;
    [SerializeField] private Transform _edgesContainer;
    [SerializeField] private Transform _nodesContainer;
    [Space]
    [SerializeField] private Single _singlePrefab;
    [SerializeField] private Double _doublePrefab;

    private HashSet<SO_GraphNode> _startNodes;
    private HashSet<SO_GraphEdge> _edges;
    private HashSet<SO_GraphNode> _nodes;

    private List<GameObject> _uiStartNodes;
    private List<GameObject> _uiEdges;
    private List<GameObject> _uiNodes;

    void Start()
    {
        Assert.IsNotNull(_graph, "Missing Graph!");
        Assert.IsNotNull(_graph, "Missing category of control variables!");

        _startNodes = new(START_SIZE);
        _edges = new(_graph.edges.Count);
        _nodes = new(_graph.nodes.Count);

        _uiStartNodes = new(START_SIZE);
        _uiEdges = new(_graph.edges.Count);
        _uiNodes = new(_graph.nodes.Count);

        GenerateSubgraph();
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
        _edges.Clear();

        ClearUIElements(_uiStartNodes);
        ClearUIElements(_uiEdges);
        ClearUIElements(_uiNodes);

        GenerateSubgraph();

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

        var startNodes = _graph.nodes.Where(n => n.category == _controlVarCategory).ToArray();
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
            GetSubgraphRecursive(node, _nodes, _edges);
        }

        _startNodes = _startNodes.OrderBy(n => n.index).ToHashSet();
        _edges = _edges.OrderBy(n => n.from.id).ToHashSet();
        _nodes = _nodes.OrderBy(n => n.id).ToHashSet();

        Debug.Log($"Subgraph size: Edges '{_edges.Count}' - Nodes '{_nodes.Count}'");

        foreach(var startNode in _startNodes)
        {
            var uiNode = Instantiate(_singlePrefab, _startNodesContainer);
            uiNode.Text.text = startNode.id;
            _uiStartNodes.Add(uiNode.gameObject);
        }

        foreach(var edge in _edges)
        {
            var uiEdge = Instantiate(_doublePrefab, _edgesContainer);
            uiEdge.LeftText.text = edge.from.id;
            uiEdge.RightText.text = edge.to.id;
            _uiEdges.Add(uiEdge.gameObject);
        }

        foreach(var node in _nodes)
        {
            var uiNode = Instantiate(_singlePrefab, _nodesContainer);
            uiNode.Text.text = node.id;
            _uiNodes.Add(uiNode.gameObject);
        }
    }

    private void GetSubgraphRecursive(SO_GraphNode node, HashSet<SO_GraphNode> nodes, HashSet<SO_GraphEdge> edges)
    {
        if(node == null || nodes == null || edges == null)
            return;

        foreach(SO_GraphEdge edge in _graph.edges.Where(e => e.from == node))
        {
            GetSubgraphRecursive(edge.to, nodes, edges);
            nodes.Add(edge.to);
            edges.Add(edge);
        }
    }
}
