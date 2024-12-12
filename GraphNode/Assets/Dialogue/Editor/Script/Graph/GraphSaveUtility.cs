using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphSaveUtility
{
    private View _targetGraphView;
    private GraphEditorWindow _graph = new GraphEditorWindow();
    private GraphContainer _containerCache;
    private List<Edge> _edges => _targetGraphView.edges.ToList();
    
    public readonly Vector2 defaultNodeSize = new Vector2(150, 200);
    private List<GraphNode> nodes => _targetGraphView.nodes.ToList().Cast<GraphNode>().ToList();
    
    public static GraphSaveUtility GetInstance(View targetGraphView)
    {
        return new GraphSaveUtility
        {
            _targetGraphView = targetGraphView
        };
    }

    #region Save/Load Graph
    public void SaveGraph(string fileName)
    {
        Debug.Log(_edges);
        if (!_edges.Any()) return;

        var dialogueContainer = ScriptableObject.CreateInstance<GraphContainer>();

        // Sauvegarder les liens entre nodes
        var connectedPorts = _edges.Where(x => x.input.node != null).ToArray();
        foreach (var port in connectedPorts)
        {
            var outputNode = port.output.node as GraphNode;
            var inputNode = port.input.node as GraphNode;

            dialogueContainer._nodeLinks.Add(new NodeLinkData
            {
                _baseNodeID = outputNode._nodeID,
                _portName = port.output.portName,
                _targetNodeID = inputNode._nodeID
            });
        }

        // Sauvegarder les données des nodes
        foreach (var node in nodes.Where(node => !node._isStartNode))
        {
            var nodeData = new NodeData
            {
                _nodeID = node._nodeID,
                _nodeName = node._nodeName,
                _position = node.GetPosition().position,
            };

            dialogueContainer._nodeData.Add(nodeData);
        }
        // Sauvegarder dans un asset Unity
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");

        AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();
    }

    public void LoadGraph(string fileName)
    {
        _containerCache = Resources.Load<GraphContainer>(fileName);
        if (_containerCache == null)
        {
            EditorUtility.DisplayDialog("File Not Found", "Target dialogue graph file does not exist!", "OK");
            return;
        }

        ClearGraph();
        CreateNodes();
        ConnectNodes();
    }

    private void ClearGraph()
    {
        nodes.Find(x => x._isStartNode)._nodeID = _containerCache._nodeLinks[0]._baseNodeID;

        foreach (var node in nodes)
        {
            if (node._isStartNode) continue;

            _edges.Where(x => x.input.node == node).ToList().ForEach(edge => _targetGraphView.RemoveElement(edge));
            _targetGraphView.RemoveElement(node);
        }
    }

    private void CreateNodes()
    {
        foreach (var nodeData in _containerCache._nodeData)
        {
            // var tempNode = _graph.SetNewNode(nodeData._nodeName, nodeData._properties, nodeData._propertiesValue);
            // tempNode._nodeID = nodeData._nodeID;
            //
            // _targetGraphView.AddElement(tempNode);
            //
            // tempNode.SetPosition(new Rect(nodeData._position, defaultNodeSize));
            //
            // var nodePorts = _containerCache._nodeLinks.Where(x => x._baseNodeID == nodeData._nodeID).ToList();
            // nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x._portName));
        }
    }

    private void ConnectNodes()
    {
        for (var i = 0; i < nodes.Count; i++)
        {
            var connections = _containerCache._nodeLinks.Where(x => x._baseNodeID == nodes[i]._nodeID).ToList();
            for (var j = 0; j < connections.Count; j++)
            {
                var targetNodeID = connections[j]._targetNodeID;
                var targetNode = nodes.First(x => x._nodeID == targetNodeID);
                LinkNodes(nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);
                
                targetNode.SetPosition(new Rect(_containerCache._nodeData.First(x => x._nodeID == targetNodeID)._position, defaultNodeSize));
            }
        }
    }

    private void LinkNodes(Port output, Port input)
    {
        var tempEdge = new Edge
        {
            output = output,
            input = input
        };

        tempEdge?.input.Connect(tempEdge);
        tempEdge?.output.Connect(tempEdge);
        _targetGraphView.Add(tempEdge);
    }
    #endregion
}