using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ProximityPrompt : MonoBehaviour
{
    [SerializeField] IInteractable interactable;
    [SerializeField] UnityEvent onEnter;
    [SerializeField] UnityEvent onExit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        onEnter.Invoke();

        interactable.CanInteract = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player") return;

        onExit.Invoke();

        interactable.CanInteract = false;
    }
}
