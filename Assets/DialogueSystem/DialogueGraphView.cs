using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView
{
    public DialogueGraphView()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        
        AddElement(GenerateStartingNode());
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        List<Port> compatiblePorts = new List<Port>();

        foreach (var port in ports)
        {
            if (startPort != port)
                compatiblePorts.Add(port);
        }

        return compatiblePorts;
    }

    private Port GeneratePort(DialogueNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
    }

    private DialogueNode GenerateStartingNode()
    {
        DialogueNode node = new DialogueNode("Start",Guid.NewGuid().ToString(),"Starting Node", true);

        Port generatedPort = GeneratePort(node, Direction.Output);
        generatedPort.portName = "Next";
        node.outputContainer.Add(generatedPort);
        
        node.RefreshExpandedState();
        node.RefreshPorts();
        
        node.SetPosition( new Rect(100, 200, 100, 150));
        return node;
    }

    public void CreateNode(string nodeTitle)
    {
        AddElement(CreateDialogueNode(nodeTitle));
    }

    public DialogueNode CreateDialogueNode(string nodeTitle)
    {
        DialogueNode node = new DialogueNode(nodeTitle, Guid.NewGuid().ToString(), nodeTitle, false);

        Port inputPort = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        node.inputContainer.Add(inputPort);

        Button button = new Button(() => { AddChoicePort(node); });
        button.text = "Add Choice";
        node.titleContainer.Add(button);

        TextField textField = new TextField("");
        textField.RegisterValueChangedCallback(e =>
        {
            node.text = e.newValue;
            node.title = e.newValue;
        });
        textField.SetValueWithoutNotify(node.title);
        node.mainContainer.Add(textField);
        
        node.RefreshExpandedState();
        node.RefreshPorts();
        node.SetPosition(new Rect(Vector2.zero, new Vector2(150, 200)));

        return node;
    }

    public void AddChoicePort(DialogueNode node, string overriddenPortName = "")
    {
        Port generatedPort = GeneratePort(node, Direction.Output);

        Label oldLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(oldLabel);
        
        int outputPortCount = node.outputContainer.Query("connector").ToList().Count;
        string portName = string.IsNullOrEmpty(overriddenPortName) ? "Choice " + outputPortCount : overriddenPortName;
        
        TextField textField = new TextField();
        textField.name = string.Empty;
        textField.value = portName;
        textField.RegisterValueChangedCallback(e => generatedPort.portName = e.newValue);
        var flexibleSpace = new VisualElement();
        flexibleSpace.style.flexGrow = 1;
        flexibleSpace.Add(textField);
        generatedPort.contentContainer.Add(new Label("  "));
        generatedPort.contentContainer.Add(flexibleSpace);
        
        Button deleteButton = new Button(() => RemovePort(node, generatedPort));
        deleteButton.text = "X";
        generatedPort.contentContainer.Add(deleteButton);

        generatedPort.portName = portName;
        node.outputContainer.Add(generatedPort);
        node.RefreshExpandedState();
        node.RefreshPorts();
    }

    private void RemovePort(DialogueNode node, Port generatedPort)
    {
        var targetEdge = edges.ToList().Where(x =>
            x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);

        if (targetEdge.Any())
        {
            Edge edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(edge);
        }
        
        node.outputContainer.Remove(generatedPort);
        node.RefreshPorts();
        node.RefreshExpandedState();
    }
}
