using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSequence", menuName = "DialogueSequence")]
public class DialogueSequenceList : ScriptableObject
{
    [System.Serializable]
    public struct DialogueSequenceItem
    {
        public string Author;
        public string Text;
        public float Duration;
        public AudioClip AudioClip;
        public string option1;
        public string option2;
    }

    public List<DialogueSequenceItem> dialogueSequenceList;
}
