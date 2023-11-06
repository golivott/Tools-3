using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;

public class DialogueGraph : EditorWindow
{
    private DialogueGraphView graphView;
    private string fileName = "New Dialogue";
    
    [MenuItem("Tools/Dialogue Editor")]
    public static void OpenDialogueEditor()
    {
        DialogueGraph window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent("Dialogue Editor");
    }

    private void OnEnable()
    {
        // Create the graph
        graphView = new DialogueGraphView();
        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
        
        // Create toolbar
        Toolbar toolbar = new Toolbar();

        // File Text field
        TextField fileNameField = new TextField("File Name:");
        fileNameField.SetValueWithoutNotify(fileName);
        fileNameField.MarkDirtyRepaint();
        fileNameField.RegisterValueChangedCallback(e => fileName = e.newValue);
        toolbar.Add(fileNameField);

        // Save button
        Button saveButton = new Button(() => UpdateData(true));
        saveButton.text = "Save Data";
        toolbar.Add(saveButton);
        
        // Load button
        Button loadButton = new Button(() => UpdateData(false));
        loadButton.text = "Load Data";
        toolbar.Add(loadButton);
        
        // Create Node button
        Button nodeCreateButton = new Button(() => { graphView.CreateNode("Dialogue Node"); });
        nodeCreateButton.text = "Create Node";
        toolbar.Add(nodeCreateButton);
        
        rootVisualElement.Add(toolbar);
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(graphView);
    }

    private void UpdateData(bool save)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            EditorUtility.DisplayDialog("Invalid file name", "Enter a valid file name", "OK");
            return;
        }
        
        List<Edge> edges = graphView.edges.ToList();
        List<DialogueNode> nodes = graphView.nodes.ToList().Cast<DialogueNode>().ToList();

        if (save) // Save dialogue
        {
            if(edges.Count == 0) return;

            DialogueModel dialogue = ScriptableObject.CreateInstance<DialogueModel>();

            Edge[] ports = edges.Where(x => x.input.node != null).ToArray();
            foreach (Edge edge in ports)
            {
                DialogueNode output = edge.output.node as DialogueNode;
                DialogueNode input = edge.input.node as DialogueNode;
                
                dialogue.linkData.Add(new LinkData(output.GUID, edge.output.portName, input.GUID));
            }

            foreach (DialogueNode node in nodes.Where(node => !node.startingNode))
            {
                dialogue.nodeData.Add(new NodeData(node.GUID,node.text,node.GetPosition().position));
            }
            
            AssetDatabase.CreateAsset(dialogue, "Assets/DialogueSystem/Saved/" + fileName + ".asset");
            AssetDatabase.SaveAssets();
        }
        else // Load dialogue
        {
             DialogueModel dialogue = AssetDatabase.LoadAssetAtPath<DialogueModel>("Assets/DialogueSystem/Saved/" + fileName + ".asset");
            
            if (dialogue == null)
            {
                EditorUtility.DisplayDialog("File not found", "File does not exist", "OK");
                return;
            }

            // Clear graph
            nodes.Find(x => x.startingNode).GUID = dialogue.linkData[0].baseGuid;
            foreach (DialogueNode node in nodes)
            {
                if(node.startingNode) continue;

                edges.Where(x => x.input.node == node).ToList().ForEach(edge => graphView.RemoveElement(edge));
                graphView.RemoveElement(node);
            }

            // Create nodes
            foreach (NodeData node in dialogue.nodeData)
            {
                DialogueNode newNode = graphView.CreateDialogueNode(node.text);
                newNode.GUID = node.GUID;
                graphView.AddElement(newNode);

                dialogue.linkData.Where(x => x.baseGuid == node.GUID).ToList().ForEach(x => graphView.AddChoicePort(newNode, x.portName));
                
            }
            
            // Create links
            nodes = graphView.nodes.ToList().Cast<DialogueNode>().ToList();
            foreach (DialogueNode node in nodes)
            {
                List<LinkData> links = dialogue.linkData.Where(x => x.baseGuid == node.GUID).ToList();
                for (int i = 0;i < links.Count; i++)
                {
                    string targetNodeGuid = links[i].targetGuid;
                    DialogueNode targetNode = nodes.First(x => x.GUID == targetNodeGuid);
                    
                    Port output = node.outputContainer[i].Q<Port>();
                    Port input = (Port) targetNode.inputContainer[0];
                    Edge newEdge = new Edge();
                    newEdge.output = output;
                    newEdge.input = input;
                    newEdge.input.Connect(newEdge);
                    newEdge.output.Connect(newEdge);
                    graphView.Add(newEdge);
                    
                    targetNode.SetPosition(
                        new Rect(dialogue.nodeData.First(x => x.GUID == targetNodeGuid).position, 
                            new Vector2(150, 200)
                            ));
                }
            }
        }
    }
}
