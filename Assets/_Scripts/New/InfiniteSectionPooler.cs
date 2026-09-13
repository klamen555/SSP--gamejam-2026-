using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pools road/track "sections" for an infinite driving game.
/// Spawns a new section each time the car passes through this trigger,
/// recycles the oldest section (no Destroy/Instantiate churn), and can
/// be stopped or have its direction reversed via a special trigger event.
/// </summary>
public class InfiniteSectionPooler : MonoBehaviour
{
    [Header("Section prefabs")]
    [SerializeField] private List<GameObject> sectionPrefabs = new List<GameObject>();
    [SerializeField] private GameObject specialSectionPrefab; // e.g. your "PennySection"
    [SerializeField] private GameObject startSectionPrefab;   // forced as the very first section

    [Header("Layout / movement")]
    [SerializeField] private float sectionLength = 234f;
    [SerializeField] private float speed = 5f;
    [SerializeField] private int activeSectionCount = 5; // how many sections stay "alive" at once

    private readonly List<GameObject> activeSections = new List<GameObject>();
    private readonly Queue<GameObject> inactivePool = new Queue<GameObject>();

    // -1 = moving toward -Z (default "forward"), +1 = reversed
    private float direction = -1f;
    private bool isStopped = false;

    // Set this externally (e.g. from the special trigger event) to force
    // a specific prefab (like specialSectionPrefab) to spawn next.
    private GameObject nextSectionOverride;

    private void Start()
    {
        // Force a specific prefab (e.g. a runway/start section) as the very first spawn.
        if (startSectionPrefab != null)
        {
            nextSectionOverride = startSectionPrefab;
        }

        // Pre-fill the track so there's something in front of the car
        // as soon as the scene starts, instead of waiting for the first trigger.
        for (int i = 0; i < activeSectionCount; i++)
        {
            SpawnNextSection();
        }
    }

    /// <summary>
    /// Called by a SectionTrigger (on an individual section) when the car
    /// passes through it. The manager itself holds no collider.
    /// </summary>
    public void HandleCarPassedTrigger()
    {
        SpawnNextSection();
        RecycleOldestSectionIfNeeded();
    }

    private void SpawnNextSection()
    {
        GameObject prefab = nextSectionOverride != null
            ? nextSectionOverride
            : sectionPrefabs[Random.Range(0, sectionPrefabs.Count)];

        nextSectionOverride = null; // consume the override once used

        float lastZ = activeSections.Count > 0
            ? activeSections[activeSections.Count - 1].transform.position.z
            : transform.position.z;

        // Sections spawn "ahead" relative to current travel direction.
        Vector3 newPosition = new Vector3(0f, 0f, lastZ + sectionLength * -direction);

        GameObject section = GetFromPoolOrInstantiate(prefab, newPosition);
        activeSections.Add(section);
    }

    private GameObject GetFromPoolOrInstantiate(GameObject prefab, Vector3 position)
    {
        GameObject section;

        if (inactivePool.Count > 0)
        {
            section = inactivePool.Dequeue();
            section.transform.SetPositionAndRotation(position, Quaternion.identity);
            section.SetActive(true);
        }
        else
        {
            section = Instantiate(prefab, position, Quaternion.identity);
        }

        // Each section owns its own trigger, typically on a child object; point it back at this manager.
        SectionTrigger trigger = section.GetComponentInChildren<SectionTrigger>();
        if (trigger != null)
        {
            trigger.Initialize(this);
        }
        else
        {
            Debug.LogWarning($"Section prefab '{prefab.name}' has no SectionTrigger component (checked children too).", section);
        }

        return section;
    }

    private void RecycleOldestSectionIfNeeded()
    {
        if (activeSections.Count <= activeSectionCount) return;

        GameObject oldest = activeSections[0];
        activeSections.RemoveAt(0);
        oldest.SetActive(false);
        inactivePool.Enqueue(oldest);
    }

    private void Update()
    {
        if (isStopped) return;

        Vector3 movement = new Vector3(0f, 0f, direction) * speed * Time.deltaTime;
        for (int i = 0; i < activeSections.Count; i++)
        {
            activeSections[i].transform.position += movement;
        }
    }

    // --- Hook these up to your "special trigger event" ---

    /// <summary>
    /// Call from the special trigger. Optionally reverse travel direction
    /// and/or force a specific section (e.g. specialSectionPrefab) to spawn next.
    /// </summary>
    public void OnSpecialTriggerEvent(bool reverseDirection, GameObject forcedNextSection = null)
    {
        if (reverseDirection)
            direction *= -1f;

        if (forcedNextSection != null)
            nextSectionOverride = forcedNextSection;

        isStopped = false;
    }

    public void StopMovement() => isStopped = true;

    public void ResumeMovement() => isStopped = false;
}