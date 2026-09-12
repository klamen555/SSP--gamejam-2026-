using UnityEngine;

public class DoorScript : MonoBehaviour
{
   public ProximityPromptScript ProximityPromptScript;
   [SerializeField] private Animator animator;

   [SerializeField] private AudioSource audioSource;

   void OpenDoor()
   {
    animator.SetTrigger("Open");
    audioSource.Play();
   }

   void OnEnable()
   {
    ProximityPromptScript.IsTriggered += OpenDoor;
   }

   void OnDisable()
   {
    ProximityPromptScript.IsTriggered -= OpenDoor;
   }
}
