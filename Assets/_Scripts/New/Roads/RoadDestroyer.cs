using UnityEngine;

public class RoadDestroyer : MonoBehaviour
{
    [SerializeField] GameObject Parent;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) GameObject.FindObjectOfType<RoadManager>().DestroyItem(Parent);
    }
}
