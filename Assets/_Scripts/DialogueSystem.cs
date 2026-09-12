using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    [System.Serializable]
    public struct DialogueSequence
    {
        public string Author;
        public string Text;
        public float Duration;
        public AudioClip AudioClip;
    }

    [SerializeField] TextMeshProUGUI dialogueObject;
    [SerializeField] AudioSource audioSource;

    public List<DialogueSequence> dialogueSequenceList;

    private IEnumerator DialogueWriter()
    {
        foreach(DialogueSequence sequence in dialogueSequenceList)
        {
            if (sequence.AudioClip != null)
            {
                audioSource.clip = sequence.AudioClip;
                audioSource.Play();
            }
            dialogueObject.text = $"[{sequence.Author}]: {sequence.Text}";
            yield return new WaitForSeconds(sequence.Duration);
        }
    }

    public void TriggerDialogue()
    {

    }
}
