using UnityEngine;

public class StartGame : MonoBehaviour
{
    [SerializeField] RoadSectionSpawner spawner;
    [SerializeField] GameObject start_section;

    private void Awake()
    {
        spawner.QueueSpecificSection(start_section);
    }
}
