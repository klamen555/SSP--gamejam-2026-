using UnityEngine;

public class CarScript : MonoBehaviour
{
    [SerializeField] RoadManager RoadManager;
    [SerializeField] ProximityPromptScript ProximityPromptScript;
    [SerializeField] PlayerMovement movement;
    [SerializeField] float carSpeed;
    [SerializeField] Vector3 sitPos;
    [SerializeField] Vector3 normalPos;
    public void StartRide()
    {
        RoadManager.SetTargetSpeed(carSpeed);
        ProximityPromptScript.IsVisible = false;
        normalPos = movement.transform.position;
        movement.transform.position = sitPos;
        movement.enabled = !movement.enabled;

        Camera.main.transform.rotation = Quaternion.identity;

        Cursor.visible = !Cursor.visible;

        if (Cursor.lockState == CursorLockMode.Locked) Cursor.lockState = CursorLockMode.None;
        else Cursor.lockState = CursorLockMode.Locked;
    }
    public void StopRide()
    {
        RoadManager.SetTargetSpeed(0);
        movement.transform.position = normalPos;
        movement.enabled = !movement.enabled;

        Cursor.visible = !Cursor.visible;

        if (Cursor.lockState == CursorLockMode.Locked) Cursor.lockState = CursorLockMode.None;
        else Cursor.lockState = CursorLockMode.Locked;
    }
}
