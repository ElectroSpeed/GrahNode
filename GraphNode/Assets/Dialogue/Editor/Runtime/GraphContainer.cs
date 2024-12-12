using System.Collections.Generic;
using UnityEngine;
    
[CreateAssetMenu(fileName = "GraphContainer", menuName = "Scriptable Objects/GraphContainer")]
public class GraphContainer : ScriptableObject
{
    [HideInInspector] public List<NodeLinkData> _nodeLinks = new List<NodeLinkData>();
    public List<NodeData> _nodeData = new List<NodeData>();
}

