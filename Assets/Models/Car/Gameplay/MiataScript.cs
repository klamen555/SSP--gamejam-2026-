using UnityEngine;

public class MiataScript : MonoBehaviour
{
    public ProximityPromptScript prox;
    public Transform RB;
    public Transform SitPoint;
    public Transform EscapePoint;
    public Transform PlayerCamera;
    //public PlayerController playerController;

    private bool isSitting;

    private void SitInMiata()
    {
        RB.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        MeshRenderer PlayerRenderer = RB.GetComponent<MeshRenderer>();
        PlayerRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        RB.SetParent(SitPoint);
        
        RB.localPosition = Vector3.zero;

        RB.localRotation = Quaternion.Euler(0, 0, 0);
        PlayerCamera.localRotation = Quaternion.Euler(0, 0, 0);

        prox.IsVisible = false;
        //playerController.CanWalk = false;
        //playerController.applyLookLimits = true;
        isSitting = true;
    }

    private void ExitMiata()
    {
        isSitting = false;
        
        RB.SetParent(null);
        RB.SetPositionAndRotation(EscapePoint.position, EscapePoint.rotation);
        
        RB.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        MeshRenderer PlayerRenderer = RB.GetComponent<MeshRenderer>();
        PlayerRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        
        prox.IsVisible = true;
        //playerController.CanWalk = true;
        //playerController.applyLookLimits = false;
    }

    private void Update()
    {
        if (isSitting && Input.GetKeyDown(KeyCode.E))
        {
            ExitMiata();
        }
    }

    void OnEnable()
    {
        prox.IsTriggered += SitInMiata;
    }

    void OnDisable()
    {
        prox.IsTriggered -= SitInMiata;
    }
}