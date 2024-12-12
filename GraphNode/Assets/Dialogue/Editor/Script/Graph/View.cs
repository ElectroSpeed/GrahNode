using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class View : GraphView
{
    public View()
    {
        InitializeStyleSheet();
        ConfigureZoom();
        ConfigureManipulators();
        ConfigureGridBackground();
        AddElement(CreateStartNode());
    }

    private void InitializeStyleSheet()
    {
        styleSheets.Add(Resources.Load<StyleSheet>("GraphView"));
    }

    private void ConfigureZoom()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
    }

    private void ConfigureManipulators()
    {
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
    }

    private void ConfigureGridBackground()
    {
        var gridBackground = new GridBackground();
        Insert(0, gridBackground);
        gridBackground.StretchToParentSize();
    }

    private GraphNode CreateStartNode()
    {
        var startNode = new GraphNode
        {
            title = "Start",
            _nodeID = Guid.NewGuid().ToString(),
            _nodeName = "Start Node",
            _isStartNode = true
        };

        var outputPort = CreatePort(startNode, Direction.Output);
        outputPort.portName = "Next";
        startNode.outputContainer.Add(outputPort);

        ConfigureStartNodeCapabilities(startNode);
        startNode.SetPosition(new Rect(100, 200, 100, 150));

        return startNode;
    }

    private void ConfigureStartNodeCapabilities(GraphNode startNode)
    {
        startNode.capabilities &= ~Capabilities.Movable;
        startNode.capabilities &= ~Capabilities.Deletable;

        startNode.RefreshPorts();
        startNode.RefreshExpandedState();
    }

    public Port CreatePort(GraphNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(port => startPort != port && startPort.node != port.node).ToList();
    }
    
    
    # region Choice Port
        public void AddChoicePort(GraphNode node, string overridePortName = "")
        {
            var choicePort = CreatePort(node, Direction.Output);

            var outputPortCount = node.outputContainer.Query("connector").ToList().Count;
            var choicePortName = string.IsNullOrEmpty(overridePortName) ? $"Choice {outputPortCount + 1}" : overridePortName;

            ConfigureChoicePort(choicePort, choicePortName, node);

            node.outputContainer.Add(choicePort);
            node.RefreshPorts();
            node.RefreshExpandedState();
        }

        private void ConfigureChoicePort(Port choicePort, string choicePortName, GraphNode node)
        {
            var choicePortNameTextField = new TextField { value = choicePortName };
            choicePort.contentContainer.Add(choicePortNameTextField);

            choicePortNameTextField.RegisterValueChangedCallback(evt =>
            {
                choicePort.portName = evt.newValue;
            });

            var deleteButton = new Button(() => RemoveChoicePort(node, choicePort)) { text = "X" };
            choicePort.contentContainer.Add(deleteButton);

            choicePort.portName = choicePortName;
        }

        private void RemoveChoicePort(GraphNode node, Port choicePort)
        {
            var connectedEdges = edges.ToList().Where(edge => edge.output == choicePort);

            foreach (var edge in connectedEdges)
            {
                edge.input.Disconnect(edge);
                RemoveElement(edge);
            }

            node.outputContainer.Remove(choicePort);
            node.RefreshPorts();
            node.RefreshExpandedState();
        }
     # endregion
}