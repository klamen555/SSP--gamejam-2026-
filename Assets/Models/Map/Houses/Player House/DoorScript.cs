using UnityEngine;

public class DoorScript : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private AudioSource audioSource;

    public void OpenDoor()
    {
        animator.SetTrigger("Open");
        audioSource.Play();
    }
}
