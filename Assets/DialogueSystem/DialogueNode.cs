using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DialogueNode : Node
{
    public string GUID;

    public string text;

    public bool startingNode = false;

    public DialogueNode(string title, string GUID, string text, bool startingNode)
    {
        base.title = title;
        this.GUID = GUID;
        this.text = text;
        this.startingNode = startingNode;
    }
}
