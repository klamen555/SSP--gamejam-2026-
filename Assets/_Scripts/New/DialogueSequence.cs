using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum DialogueNodeType
{
    Line,
    Choice
}

[System.Serializable]
public class DialogueNode
{
    public DialogueNodeType type = DialogueNodeType.Line;

    [Header("Line")]
    public string speakerName;
    [TextArea(2, 5)] public string text;

    [Header("Choice")]
    public string optionAText = "Option A";
    public UnityEvent onOptionASelected;
    [Tooltip("Node index to jump to after this option. Leave -1 to just continue to the next node.")]
    public int optionANextIndex = -1;

    public string optionBText = "Option B";
    public UnityEvent onOptionBSelected;
    [Tooltip("Node index to jump to after this option. Leave -1 to just continue to the next node.")]
    public int optionBNextIndex = -1;
}

[CreateAssetMenu(fileName = "New Dialogue Sequence", menuName = "Dialogue/Dialogue Sequence")]
public class DialogueSequence : ScriptableObject
{
    public List<DialogueNode> nodes = new List<DialogueNode>();
}
