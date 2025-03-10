using Graph;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Company : MonoBehaviour
{
    const int START_SIZE = 4;

    [SerializeField] private SO_Graph _graph;
    [SerializeField] private string _controlVarName;
    [Header("Runtime")]
    [SerializeField] private HashSet<SO_GraphNode> _startNodes;
    [SerializeField] private HashSet<SO_GraphNode> _nodes;
    [SerializeField] private HashSet<SO_GraphEdge> _edges;

    void Start()
    {
        SO_GraphCategory category = _graph.categories.FirstOrDefault(c => c.description.Equals(_controlVarName));

        if(category != null)
        {
            Debug.Log($"Missing Category for name: '{_controlVarName}'");
            return;
        }

        _startNodes = new(START_SIZE);
        _nodes = new(_graph.nodes.Count);
        _edges = new(_graph.edges.Count);

        var startNodes = _graph.nodes.Where(n => n.category == category);
        int startNodeAmount = startNodes.Count();

        for(int i = 0; i < START_SIZE; i++)
        {

        }
    }
}
