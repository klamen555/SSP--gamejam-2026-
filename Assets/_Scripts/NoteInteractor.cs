using UnityEngine;

public class NoteInteractor : MonoBehaviour
{
    public void InspectPaper()
    {
        PlayerMovement movement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        movement.enabled = !movement.enabled;

        Cursor.visible = !Cursor.visible;

        if (Cursor.lockState == CursorLockMode.Locked) Cursor.lockState = CursorLockMode.None;
        else Cursor.lockState = CursorLockMode.Locked;
    }
}
