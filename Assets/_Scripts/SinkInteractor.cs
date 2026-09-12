using UnityEngine;

public class SinkInteractor : MonoBehaviour
{
    public void FillWaterCup(InteractorSystem interactor)
    {
        GameObject currentlyHolding = interactor.currentlyHolding;

        if (currentlyHolding == null) return;

        if (currentlyHolding.GetComponent<KonviceInteractor>() != null)
        {
            currentlyHolding.GetComponent<KonviceInteractor>().FillWater();
        }
    }
}
