using UnityEngine;

namespace Graph
{
    public class SO_GraphNode : ScriptableObject
    {
        public SO_GraphCategory category;
        public int index;
        public string description;

        public string id => $"{category.id}{index}";
    }
}
