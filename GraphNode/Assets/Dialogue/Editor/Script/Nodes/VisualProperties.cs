using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class VisualProperties
{
    #region Variables
    
    public void BindProperty(GraphNode node, string key, VisualElement field)
    {
        // switch (field)
        // {
        //     case TextField textField:
        //         textField.RegisterValueChangedCallback(evt =>
        //         {
        //             node._properties[key] = evt.newValue;
        //         });
        //         break;
        //
        //     case EnumField enumField:
        //         enumField.RegisterValueChangedCallback(evt =>
        //         {
        //             node._properties[key] = evt.newValue;
        //         });
        //         break;
        //
        //     case ObjectField objectField:
        //         objectField.RegisterValueChangedCallback(evt =>
        //         {
        //             node._properties._nodeProperties[] = evt.newValue;
        //         });
        //         break;
        // }
    }
    
    public VisualElement AddSpace(GraphNode node, int length)
    {
        var space = new VisualElement();
        space.style.flexBasis = length;
        node.extensionContainer.Add(space);
        return space;
    }
    
    public Label AddHeaderField(GraphNode node, string headerText)
    {
        var header = new Label(headerText);
        header.style.alignSelf = Align.Center;
        header.style.unityFontStyleAndWeight = FontStyle.Bold;
        node.extensionContainer.Add(header);
        return header;
    }

    public Label AddLabelField(GraphNode node, string labelText)
    {
        var label = new Label(labelText);
        node.extensionContainer.Add(label);
        return label;
    }

    public TextField AddTextField(GraphNode node)
    { 
        var textField = new TextField(string.Empty);
        node.extensionContainer.Add(textField);
        return textField;
    }

    public ObjectField AddSpriteField(GraphNode node)
    {
        var objectField = new ObjectField
        {
            objectType = typeof(Sprite),
        };
        node.extensionContainer.Add(objectField);
        return objectField;
    }

    public void DrawSpriteField(GraphNode node, ObjectField objectField)
    {
        var spritePreview = new Image
        {
            style = { maxWidth = 100, maxHeight = 100, flexDirection = FlexDirection.Column, alignItems = Align.Center }
        };
        node.extensionContainer.Add(spritePreview);

        objectField.RegisterValueChangedCallback(evt =>
        {
            spritePreview.sprite = evt.newValue as Sprite;
            Debug.Log($"Sprite updated to: {(evt.newValue as Sprite)?.name}");
        });
    }
    #endregion
}