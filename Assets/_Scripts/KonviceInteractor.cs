using UnityEngine;

public class KonviceInteractor : MonoBehaviour
{
    [SerializeField] float maxCapacity;
    [SerializeField] float currentCapacity;
    [SerializeField] float capacityPerPour;
    [SerializeField] Animator animator;

    public void Water(FlowerStats flowerStats)
    {
        animator.Rebind();

        if (currentCapacity == 0f) return;

        if ((currentCapacity - capacityPerPour) < 0)
        {
            flowerStats.waterLevel += currentCapacity;
            currentCapacity = 0;
            animator.Play("WateringFlower");
        }
        else
        {
            currentCapacity -= 3f;
            animator.Play("WateringFlower");
            flowerStats.waterLevel += 3f;
        }
    }
}
