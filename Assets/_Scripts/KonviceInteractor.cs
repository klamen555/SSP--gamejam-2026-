using UnityEngine;

public class KonviceInteractor : MonoBehaviour
{
    [SerializeField] float maxCapacity;
    [SerializeField] float currentCapacity;
    [SerializeField] float capacityPerPour;
    [SerializeField] Animator animator;

    public void Water(FlowerStats flowerStats)
    {
        if ((currentCapacity - capacityPerPour) < 0)
        {
            flowerStats.waterLevel += currentCapacity;
            currentCapacity = 0;
            animator.Play("WaterPlants");
        }
        else
        {
            currentCapacity -= 3f;
            animator.Play("WaterPlants");
            flowerStats.waterLevel += 3f;
        }
    }
}
