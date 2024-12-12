using System;
using UnityEngine;

[Serializable]
public class NodeLinkData
{
    [HideInInspector] public string _portName;
    [HideInInspector] public string _baseNodeID;
    [HideInInspector] public string _targetNodeID;
}