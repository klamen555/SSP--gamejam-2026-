using UnityEngine;

public class AutomaticDoors : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] AudioClip openDoorsClip;
    [SerializeField] AudioClip closeDoorsClip;
    [SerializeField] AudioSource source;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.Play("openDoors");
            source.Stop();
            source.clip = openDoorsClip;
            source.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.Play("closeDoors");
            source.Stop();
            source.clip = closeDoorsClip;
            source.Play();
        }
    }
}
