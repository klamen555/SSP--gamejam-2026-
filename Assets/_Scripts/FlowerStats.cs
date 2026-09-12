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
    public float waterLevel;
    [Space]
    [SerializeField] Slider waterSlider;
    [SerializeField] Gradient waterSliderGradient;
    [SerializeField] Transform flowerParent;
    [SerializeField] GameObject currentFlower;

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
