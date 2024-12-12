using System;

[Serializable]
public class NodeKeyAndValueDictionary
{
    public string key;
    public ValueType type;
    public object value;
    
    public enum ValueType
    {
        String,
        Int,
        Enum,
        Sprite,
    }
}