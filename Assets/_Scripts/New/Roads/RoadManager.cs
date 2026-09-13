using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public List<GameObject> sections;
    [SerializeField] Vector3 direction;
    [SerializeField] float acceleration;
    private float currentSpeed;
    public float targetSpeed;

    public void SetTargetSpeed(float speed)
    {
        targetSpeed = speed;
    }

    public void DestroyItem(GameObject itemToDestroy)
    {
        if (sections.Contains(itemToDestroy))
        {
            sections.Remove(itemToDestroy); // 1. Remove reference from List
            Destroy(itemToDestroy);          // 2. Destroy from Scene
        }
    }

    private void Update()
    {
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * acceleration);

        foreach (var section in sections)
        {
            section.transform.position += direction.normalized * currentSpeed * Time.deltaTime;
        }
    }
}
