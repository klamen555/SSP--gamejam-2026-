using UnityEngine;

public interface IInteractable
{
    public void OnInteract();
    public bool CanInteract { get; set; }
}
