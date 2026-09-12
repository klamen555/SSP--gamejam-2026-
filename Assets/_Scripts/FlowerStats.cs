using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowerStats : MonoBehaviour
{
    [System.Serializable]
    public struct PlantStage
    {
        public int stageIndex;
        public float minimalProgressionValue;
        public GameObject prefab;
    }

    public List<PlantStage> stages = new List<PlantStage>();

    public int PlantStageIndex;
    public float progressionValue;
    public float maxWaterLevel;
    public float waterLevel;
    [Space]
    [SerializeField] Slider waterSlider;
    [SerializeField] Image sliderFill;
    [SerializeField] Gradient waterSliderGradient;
    [SerializeField] Transform flowerParent;
    [SerializeField] GameObject currentFlower;

    private void Update()
    {
        waterLevel -= 0.1f * Time.deltaTime * (PlantStageIndex + 1);

        sliderFill.color = waterSliderGradient.Evaluate(waterLevel/maxWaterLevel);
        waterSlider.value = waterLevel / maxWaterLevel;
    }

    public void WaterFlower()
    {
        InteractorSystem interactor = GameObject.FindAnyObjectByType<InteractorSystem>();

        if (interactor.currentlyHolding.GetComponent<KonviceInteractor>() != null)
        {
            interactor.currentlyHolding.GetComponent<KonviceInteractor>().Water(this);
        }
    }

    public void GrowTrigger()
    {
        if (stages[PlantStageIndex].minimalProgressionValue > progressionValue)
        {
            if (PlantStageIndex == 0)
            {
                // END
                return;
            }
            else if (PlantStageIndex > 0)
            {
                PlantStageIndex--;
            }

            GameObject newFlower = Instantiate(stages[PlantStageIndex].prefab, flowerParent);
            newFlower.transform.localPosition = currentFlower.transform.localPosition;
            Destroy(currentFlower);
            currentFlower = newFlower;
        }
        else if (stages[PlantStageIndex].minimalProgressionValue < progressionValue)
        {
            PlantStageIndex++;

            GameObject newFlower = Instantiate(stages[PlantStageIndex].prefab, flowerParent);
            newFlower.transform.localPosition = currentFlower.transform.localPosition;
            Destroy(currentFlower);
            currentFlower = newFlower;
        }
    }
}
