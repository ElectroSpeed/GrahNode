using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NodeDictionary", menuName = "Scriptable Objects/NodeDictionary")]
public class NodeDictionary : ScriptableObject
{
    public List<NodeKeyAndValueDictionary> _nodeProperties = new List<NodeKeyAndValueDictionary>();
}
