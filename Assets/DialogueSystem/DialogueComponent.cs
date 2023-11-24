using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DialogueComponent : MonoBehaviour
{
    public DialogueModel dialogue;

    public GameObject dialogueArea;
    public GameObject choicesArea;

    private NodeData currentNode;

    private void OnEnable()
    {
        currentNode = dialogue.nodeData[0];
        populateDialogue(currentNode);
    }

    private void populateDialogue(NodeData node)
    {
        // Populate dialogue
        GameObject text = new GameObject();
        TextMeshProUGUI dialogueText = text.AddComponent<TextMeshProUGUI>();
        dialogueText.name = node.text;
        dialogueText.rectTransform.SetParent( dialogueArea.transform );
        dialogueText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        dialogueText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        dialogueText.rectTransform.sizeDelta = dialogueArea.GetComponent<RectTransform>().sizeDelta;
        dialogueText.rectTransform.anchoredPosition = Vector2.zero;
        dialogueText.fontSize = 20f;

        // Populate choices
        List<LinkData> links = dialogue.linkData.Where(x => currentNode.GUID == x.baseGuid).ToList();
        if (links.Count == 0) // Final Dialogue Node
        {
            TMP_DefaultControls.Resources resources = new TMP_DefaultControls.Resources();
            GameObject button = TMP_DefaultControls.CreateButton(resources);
            button.transform.SetParent(choicesArea.transform);
            button.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            button.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            button.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            button.GetComponent<RectTransform>().sizeDelta = choicesArea.GetComponent<RectTransform>().sizeDelta;
            button.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,  0);
            button.GetComponentInChildren<TextMeshProUGUI>().fontSize = 20f;
            button.GetComponentInChildren<TextMeshProUGUI>().name = "<END>";
            
            button.GetComponent<Button>().onClick.AddListener(() => {
                ClearDialogue();
            });
        }
        
        // Create choices 
        for (int i = 0; i < links.Count; i++)
        {
            TMP_DefaultControls.Resources resources = new TMP_DefaultControls.Resources();
            GameObject button = TMP_DefaultControls.CreateButton(resources);
            button.transform.SetParent(choicesArea.transform);
            button.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            button.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            button.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            Vector2 size = choicesArea.GetComponent<RectTransform>().sizeDelta;
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x, size.y / links.Count);
            button.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,  i*size.y / links.Count - (links.Count - 1) * size.y / links.Count / 2f);
            button.GetComponentInChildren<TextMeshProUGUI>().name = links[i].portName;
            button.GetComponentInChildren<TextMeshProUGUI>().fontSize = 20f;

            string guid = links[i].targetGuid;
            button.GetComponent<Button>().onClick.AddListener(() => {
                ClearDialogue();
                currentNode = GetNode(guid);
                populateDialogue(currentNode);
            });
        }
        
        LanguageController.UpdateTextLanguage();
    }

    private void ClearDialogue()
    {
        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI text in texts)
        {
            Destroy(text.gameObject);
        }

        Button[] buttons = GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            Destroy(button.gameObject);
        }
    }

    private NodeData GetNode(string guid)
    {
        return dialogue.nodeData.First(x => x.GUID == guid);
    }
}
