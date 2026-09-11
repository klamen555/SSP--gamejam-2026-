using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ProximityPrompt : MonoBehaviour
{
    [SerializeField] MonoBehaviour interactable;
    [SerializeField] UnityEvent onEnter;
    [SerializeField] UnityEvent onExit;
    [SerializeField] Transform promptPanel;
    [SerializeField] TextMeshProUGUI promptText;

    Transform player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        onEnter.Invoke();

        interactable.GetComponent<IInteractable>().CanInteract = true;
    }

    private void OnTriggerStay(Collider other)
    {
        promptPanel.LookAt(player.position);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player") return;

        onExit.Invoke();

        interactable.GetComponent<IInteractable>().CanInteract = false;
    }
}
