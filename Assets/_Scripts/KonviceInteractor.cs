using UnityEngine;
using UnityEngine.UI;

public class KonviceInteractor : MonoBehaviour
{
    [SerializeField] float maxCapacity;
    [SerializeField] float currentCapacity;
    [SerializeField] float capacityPerPour;
    [SerializeField] Animator animator;
    [SerializeField] Slider fillSlider;
    [SerializeField] Image fillObject;
    [SerializeField] Gradient sliderGradient;

    private void Start()
    {
        UpdateSlider();
    }

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
            currentCapacity -= capacityPerPour;
            animator.Play("WateringFlower");
            flowerStats.waterLevel += capacityPerPour;
        }

        UpdateSlider();
    }
    public void FillWater()
    {
        currentCapacity = maxCapacity;

        UpdateSlider();
    }

    void UpdateSlider()
    {
        fillObject.color = sliderGradient.Evaluate(currentCapacity / maxCapacity);
        fillSlider.value = currentCapacity / maxCapacity;
    }
}
