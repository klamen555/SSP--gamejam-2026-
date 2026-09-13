using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dialogueObject;
    [SerializeField] AudioSource audioSource;

    private IEnumerator DialogueWriter(DialogueSequenceList dialogueSequenceList)
    {
        foreach(DialogueSequenceList.DialogueSequenceItem sequence in dialogueSequenceList.dialogueSequenceList)
        {
            audioSource.Stop();
            if (sequence.AudioClip != null)
            {
                audioSource.clip = sequence.AudioClip;
                audioSource.Play();
            }
            if (sequence.Author == "null") dialogueObject.text = $"{sequence.Text}";
            else dialogueObject.text = $"[{sequence.Author}]: {sequence.Text}";

            yield return new WaitForSeconds(sequence.Duration);
        }
        dialogueObject.text = "";
    }

    public void TriggerDialogue(DialogueSequenceList dialogueSequence)
    {
        StartCoroutine(DialogueWriter(dialogueSequence));
    }
}
