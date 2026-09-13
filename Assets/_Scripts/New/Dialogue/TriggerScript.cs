using UnityEngine;

public class TriggerScript : MonoBehaviour
{
    [SerializeField] DialogueSequenceList sequence;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject.FindObjectOfType<DialogueSystem>().TriggerDialogue(sequence);
            Destroy(gameObject);
        }
    }
}
