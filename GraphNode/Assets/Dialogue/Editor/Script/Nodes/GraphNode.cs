using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
public class GraphNode : Node
{
    public string _nodeID;
    public string _nodeName;
    public bool _isStartNode = false;
    
    public NodeDictionary _properties = new NodeDictionary();
}
