using UnityEngine;

public class NoteInteractor : MonoBehaviour
{
    public void InspectPaper()
    {
        PlayerMovement movement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        movement.Pause();
    }
}
