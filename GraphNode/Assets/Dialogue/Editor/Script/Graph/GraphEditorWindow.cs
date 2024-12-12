using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphEditorWindow : EditorWindow
{
    private View _graphView;
    private Toolbar _toolbar;
    private VisualProperties _visualProperties = new VisualProperties();
    private string _fileName = "New Graph";
    private readonly Vector2 _defaultNodeSize = new Vector2(150, 200);

    [MenuItem("Graph Editor/Node Graph")]
    public static void OpenGraphWindow()
    {
        var window = GetWindow<GraphEditorWindow>();
        window.titleContent = new GUIContent("Node Graph");
    }

    private void OnEnable()
    {
        GenerateGraphView();
        GenerateToolbar();
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }

    private void GenerateGraphView()
    {
        _graphView = new View
        {
            name = "Node Graph"
        };
        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    #region Toolbar
        private void GenerateToolbar()
        {
            _toolbar = new Toolbar();

            var fileNameTextField = new TextField("File Name:")
            {
                value = _fileName
            };
            fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
            _toolbar.Add(fileNameTextField);

            _toolbar.Add(new Button(() => RequestDataOperation(true)) { text = "Save Data" });
            _toolbar.Add(new Button(() => RequestDataOperation(false)) { text = "Load Data" });
            _toolbar.Add(new Button(OpenNodeConfigPanel) { text = "Create New Node Type" });

            rootVisualElement.Add(_toolbar);
        }
        
        private void RequestDataOperation(bool save)
        {
            if (string.IsNullOrEmpty(_fileName))
            {
                EditorUtility.DisplayDialog("Invalid File Name", "Please enter a valid file name.", "OK");
                return;
            }

            var saveUtility = GraphSaveUtility.GetInstance(_graphView);
            if (save)
            {
                saveUtility.SaveGraph(_fileName);
            }
            else
            {
                saveUtility.LoadGraph(_fileName);
            }
        }
    #endregion

    private void OpenNodeConfigPanel()
    {
        var popup = new VisualElement
        {
            style =
            {
                backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.9f),
                paddingTop = 20,
                paddingBottom = 20,
                paddingLeft = 20,
                paddingRight = 20,
                position = Position.Absolute,
                top = (rootVisualElement.resolvedStyle.height - 300) / 2,
                left = (rootVisualElement.resolvedStyle.width - 300) / 2,
                width = 300
            }
        };

        var nameLabel = new Label("Node Name")
        {
            style =
            {
                unityFontStyleAndWeight = FontStyle.Bold,
                unityTextAlign = TextAnchor.MiddleCenter,
                alignItems = Align.Center
            }
        };
        popup.Add(nameLabel);
        
        var nameField = new TextField();
        popup.Add(nameField);
        
        var propertyLabel = new Label("Node Properties")
        {
            style =
            {
                unityFontStyleAndWeight = FontStyle.Bold,
                unityTextAlign = TextAnchor.MiddleCenter,
                alignItems = Align.Center
            }
        };
        popup.Add(propertyLabel);
        
        var soField = new ObjectField();
        soField.objectType = typeof(NodeDictionary);
        popup.Add(soField);

        var confirmButton = new Button(() =>
        {
            var nodeName = nameField.value.Trim();
            if (!string.IsNullOrEmpty(nodeName))
            {
                CreateNode(nodeName, soField.value as NodeDictionary);
                _toolbar.Add(new Button(() => CreateNode(nodeName, soField.value as NodeDictionary)) { text = "Create " + nodeName });
            }
            rootVisualElement.Remove(popup);
        }) { text = "Create" };
        popup.Add(confirmButton);
        
        var cancelButton = new Button(() => rootVisualElement.Remove(popup)) { text = "Cancel" };
        popup.Add(cancelButton);
        
        rootVisualElement.Add(popup);
    }
    
    private void CreateNode(string nodeName, NodeDictionary _nodeProperties = null)
    {
        var newNode = new GraphNode
        {
            title = nodeName,
            _nodeID = Guid.NewGuid().ToString(),
            _nodeName = nodeName
        };

        var inputPort = _graphView.CreatePort(newNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        newNode.inputContainer.Add(inputPort);

        var addChoiceButton = new Button(() => _graphView.AddChoicePort(newNode)) { text = "New Choice" };
        newNode.titleContainer.Add(addChoiceButton);

        if (_nodeProperties != null)
        {
            AddProperties(newNode, _nodeProperties);
        }

        newNode.extensionContainer.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        newNode.RefreshPorts();
        newNode.RefreshExpandedState();
        newNode.SetPosition(new Rect(Vector2.zero, _defaultNodeSize));

        newNode.styleSheets.Add(Resources.Load<StyleSheet>("ExtensionContainerNode"));

        _graphView.AddElement(newNode);
    }
    
    private void AddProperties(GraphNode _node, NodeDictionary _nodeProperties)
    {
        foreach (var property in _nodeProperties._nodeProperties)
        {
            switch (property.type)
            {
                case NodeKeyAndValueDictionary.ValueType.String:
                    Debug.Log($"Property is a string");
                    var textField = _visualProperties.AddTextField(_node);
                    if (property.value != null)
                    {
                        textField.value = property.value.ToString();
                    }
                    break;

                case NodeKeyAndValueDictionary.ValueType.Int:
                    Debug.Log($"Property is an integer");
                    break;

                case NodeKeyAndValueDictionary.ValueType.Enum:
                    Debug.Log($"Property is an enum");
                    break;

                case NodeKeyAndValueDictionary.ValueType.Sprite:
                    Debug.Log($"Property is a sprite");
                    break;

                default:
                    Debug.LogWarning($"Unhandled property type: {property.value.GetType()}");
                    break;
            }
        }
    }
}
