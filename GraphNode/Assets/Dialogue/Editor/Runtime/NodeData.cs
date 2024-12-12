using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class NodeData
{
    [HideInInspector] public string _nodeID;
    [HideInInspector] public string _nodeName;
    [HideInInspector] public Vector2 _position;
    
    public NodeDictionary _properties = new NodeDictionary();
}