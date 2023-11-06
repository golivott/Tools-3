using System;
using UnityEngine;

[Serializable]
public class NodeData
{
    public string GUID;
    public string text;
    public Vector2 position;

    public NodeData(string guid, string text, Vector2 position)
    {
        this.GUID = guid;
        this.text = text;
        this.position = position;
    }
}
