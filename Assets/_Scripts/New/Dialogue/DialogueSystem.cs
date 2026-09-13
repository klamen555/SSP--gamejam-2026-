using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] GameObject textWindow;
    [SerializeField] GameObject choiseWindow;
    [SerializeField] TextMeshProUGUI dialogueObject;
    [SerializeField] TextMeshProUGUI button_1;
    [SerializeField] TextMeshProUGUI button_2;
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

            if (sequence.option1 != "")
            {
                choiseWindow.SetActive(true);
                textWindow.SetActive(false);

                button_1.text = sequence.option1;
                button_2.text = sequence.option2;

                yield return new WaitForSeconds(sequence.Duration);
            }
            else
            {
                choiseWindow.SetActive(false);
                textWindow.SetActive(true);
                
                if (sequence.Author == "null") dialogueObject.text = $"{sequence.Text}";
                else dialogueObject.text = $"[{sequence.Author}]: {sequence.Text}";

                yield return new WaitForSeconds(sequence.Duration);
            } 
        }
        dialogueObject.text = "";
        textWindow.SetActive(false);
        choiseWindow.SetActive(false);
    }

    public void TriggerDialogue(DialogueSequenceList dialogueSequence)
    {
        StartCoroutine(DialogueWriter(dialogueSequence));
    }
}
