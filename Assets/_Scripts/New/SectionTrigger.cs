using UnityEngine;

/// <summary>
/// Attach to each section prefab, on a Collider marked "Is Trigger"
/// (e.g. a thin box at the far end of the section). When the car
/// passes through, it tells the pooler manager to spawn the next
/// section and recycle the oldest one.
/// </summary>
public class SectionTrigger : MonoBehaviour
{
    private InfiniteSectionPooler manager;

    /// <summary>Called by the pooler right after this section is spawned or reused.</summary>
    public void Initialize(InfiniteSectionPooler owningManager)
    {
        manager = owningManager;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Car")) return;

        if (manager == null)
        {
            Debug.LogWarning("SectionTrigger fired before Initialize() was called.", this);
            return;
        }
        Debug.Log("Test1");
        manager.HandleCarPassedTrigger();
    }
}
