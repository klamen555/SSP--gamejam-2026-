using UnityEngine;

public class DestroyTest : MonoBehaviour, IInteractable
{
    public void OnInteract()
    {
        Destroy(gameObject);
    }

    public bool CanInteract { get; set; }
}
