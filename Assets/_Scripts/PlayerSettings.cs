using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "ScriptableObjects/PlayerSettings")]
public class PlayerSettings : ScriptableObject
{
    public float normalSpeed;
    public float runningSpeed;
    public float cameraSensitivity;
    [Space]
    [Header("Camera Shake")]
    public float cameraShakeFrequency;
    public float cameraShakeVerticalAmplitude;
    public float cameraShakeHorizontalAmplitude;
    public float cameraShakeSmoothing;
}
