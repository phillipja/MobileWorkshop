using Graph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Person", menuName = "Workshop/Verhandlung/Person")]
public class SO_Person : ScriptableObject
{
    public string forename;
    public string surname;
    [TextArea(5, 10)]
    public string description;
    [field: SerializeField] public List<SO_GraphNode> nodes { get; private set; } = new(5);
}
