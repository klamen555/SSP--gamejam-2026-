using UnityEngine;

/// <summary>
/// Example usage: drop this on a trigger volume (a checkpoint, hazard cue, event zone, etc.)
/// to force a specific road section to spawn next, and optionally ease the road's speed
/// up or down at the same time (e.g. slow down for a hazard, then speed back up after).
/// </summary>
[RequireComponent(typeof(Collider))]
public class RoadSectionTrigger : MonoBehaviour
{
    [Tooltip("The spawner this trigger controls.")]
    public RoadSectionSpawner spawner;

    [Tooltip("If set, this exact prefab will be queued as the next section spawned.")]
    public GameObject specificSectionPrefab;

    [Tooltip("If true, also changes the spawner's target speed when triggered.")]
    public bool changeSpeedOnTrigger = false;
    public float newTargetSpeed = 5f;

    [Tooltip("Only objects with this tag trigger the effect (e.g. the player).")]
    public string requiredTag = "Player";

    [Tooltip("Disable after firing once so it doesn't retrigger.")]
    public bool triggerOnce = true;

    private bool _hasFired;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hasFired && triggerOnce) return;
        if (spawner == null) return;
        if (!string.IsNullOrEmpty(requiredTag) && !other.CompareTag(requiredTag)) return;

        if (specificSectionPrefab != null)
        {
            spawner.QueueSpecificSection(specificSectionPrefab);
        }

        if (changeSpeedOnTrigger)
        {
            spawner.SetTargetSpeed(newTargetSpeed);
        }

        _hasFired = true;
    }
}
