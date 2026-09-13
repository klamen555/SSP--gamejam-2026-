using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns a chain of road sections that scroll past the player to simulate an endless road.
///
/// - Normally picks a random prefab from <see cref="randomSectionPrefabs"/> each time a new
///   section is needed.
/// - Call <see cref="QueueSpecificSection"/> (e.g. from a trigger zone, event, or gameplay
///   script) to force the NEXT section spawned to be a specific prefab instead of a random pick.
/// - Speed eases toward a target with separate acceleration/deceleration rates, so the road
///   speeds up and slows down gradually like a car easing on/off the throttle rather than
///   snapping instantly. Drive it with <see cref="SetTargetSpeed"/> / <see cref="Stop"/>.
/// </summary>
public class RoadSectionSpawner : MonoBehaviour
{
    [Header("Section Prefabs")]
    [Tooltip("Pool of prefabs randomly chosen from when no specific section has been queued. Each prefab needs a RoadSection component.")]
    public List<GameObject> randomSectionPrefabs = new List<GameObject>();

    [Header("Spawn Setup")]
    [Tooltip("Sections travel in this direction every frame (world space). Default: toward the player.")]
    public Vector3 moveDirection = Vector3.back;
    [Tooltip("New sections are instantiated here, ahead of the player.")]
    public Transform spawnPoint;
    [Tooltip("How far a section travels (along moveDirection) before it is despawned/recycled.")]
    public float travelDistanceBeforeDespawn = 100f;
    [Tooltip("How many sections to pre-spawn on Start so the road isn't empty at launch.")]
    public int prewarmSectionCount = 4;

    [Header("Speed & Car-like Easing")]
    [Tooltip("Current movement speed of the road (units/sec). Eases toward targetSpeed each frame.")]
    public float currentSpeed = 0f;
    [Tooltip("Speed the system is currently easing toward.")]
    public float targetSpeed = 10f;
    [Tooltip("How fast speed increases when targetSpeed > currentSpeed (units/sec^2).")]
    public float acceleration = 8f;
    [Tooltip("How fast speed decreases when targetSpeed < currentSpeed (units/sec^2). Usually higher than acceleration, like braking.")]
    public float deceleration = 14f;
    [Tooltip("Optional 0-1 easing curve applied on top of the linear ramp for a more organic start/stop feel. Leave empty for plain linear easing.")]
    public AnimationCurve easingCurve;

    private class ActiveSection
    {
        public GameObject go;
        public float length;
        public float traveled;
    }

    private readonly List<ActiveSection> _active = new List<ActiveSection>();
    private readonly Queue<GameObject> _forcedQueue = new Queue<GameObject>();

    private Vector3 _nextSpawnPosition;
    private float _distanceUntilNextSpawn;

    private void Awake()
    {
        if (spawnPoint == null)
        {
            Debug.LogWarning($"[{nameof(RoadSectionSpawner)}] No spawnPoint assigned, using this GameObject's transform instead.");
            spawnPoint = transform;
        }

        moveDirection = moveDirection.sqrMagnitude > 0f ? moveDirection.normalized : Vector3.back;
        _nextSpawnPosition = spawnPoint.position;
    }

    private void Start()
    {
        for (int i = 0; i < prewarmSectionCount; i++)
        {
            SpawnNextSection();
        }
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        UpdateSpeed(dt);
        MoveAndRecycleSections(dt);
        HandleSpawnTiming(dt);
    }

    // ---------------------------------------------------------------------
    // Public API
    // ---------------------------------------------------------------------

    /// <summary>Smoothly changes the speed the road eases toward. Call to accelerate or decelerate.</summary>
    public void SetTargetSpeed(float newTargetSpeed)
    {
        targetSpeed = newTargetSpeed;
    }

    /// <summary>Smoothly brings the road to a stop, like easing off the gas and braking.</summary>
    public void Stop()
    {
        targetSpeed = 0f;
    }

    /// <summary>Instantly sets speed with no easing. Use for resets/teleports, not normal gameplay.</summary>
    public void SetSpeedImmediate(float speed)
    {
        currentSpeed = speed;
        targetSpeed = speed;
    }

    /// <summary>
    /// Forces the NEXT section spawned to use this prefab instead of a random pick.
    /// Call it multiple times to queue several specific sections back to back.
    /// </summary>
    public void QueueSpecificSection(GameObject prefab)
    {
        if (prefab == null) return;
        _forcedQueue.Enqueue(prefab);
    }

    /// <summary>Clears any queued forced prefabs so spawning returns to purely random selection.</summary>
    public void ClearQueuedSections()
    {
        _forcedQueue.Clear();
    }

    // ---------------------------------------------------------------------
    // Internals
    // ---------------------------------------------------------------------

    private void UpdateSpeed(float dt)
    {
        if (Mathf.Approximately(currentSpeed, targetSpeed)) return;

        bool speedingUp = targetSpeed > currentSpeed;
        float rate = speedingUp ? acceleration : deceleration;

        if (easingCurve != null && easingCurve.length > 0 && targetSpeed > 0.0001f)
        {
            float t = Mathf.InverseLerp(0f, targetSpeed, currentSpeed);
            float curveMultiplier = Mathf.Max(0.01f, easingCurve.Evaluate(t));
            rate *= curveMultiplier;
        }

        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, rate * dt);
    }

    private void MoveAndRecycleSections(float dt)
    {
        float step = currentSpeed * dt;

        for (int i = _active.Count - 1; i >= 0; i--)
        {
            ActiveSection section = _active[i];
            section.go.transform.position += moveDirection * step;
            section.traveled += step;

            if (section.traveled >= travelDistanceBeforeDespawn)
            {
                DespawnSection(section);
                _active.RemoveAt(i);
            }
        }
    }

    private void HandleSpawnTiming(float dt)
    {
        _distanceUntilNextSpawn -= currentSpeed * dt;
        if (_distanceUntilNextSpawn <= 0f)
        {
            SpawnNextSection();
        }
    }

    private void SpawnNextSection()
    {
        GameObject prefab = _forcedQueue.Count > 0 ? _forcedQueue.Dequeue() : PickRandomPrefab();
        if (prefab == null) return;

        GameObject instance = Instantiate(prefab, _nextSpawnPosition, spawnPoint.rotation);

        float length = GetSectionLength(prefab, instance);

        _active.Add(new ActiveSection { go = instance, length = length, traveled = 0f });

        // Advance the spawn cursor so the next section starts exactly where this one ends.
        _nextSpawnPosition += -moveDirection * length;
        _distanceUntilNextSpawn = length;
    }

    private GameObject PickRandomPrefab()
    {
        if (randomSectionPrefabs == null || randomSectionPrefabs.Count == 0) return null;
        return randomSectionPrefabs[Random.Range(0, randomSectionPrefabs.Count)];
    }

    private float GetSectionLength(GameObject prefab, GameObject instance)
    {
        if (instance.TryGetComponent<RoadSection>(out var section))
        {
            return section.GetLength();
        }

        Debug.LogWarning($"[{nameof(RoadSectionSpawner)}] Prefab '{prefab.name}' has no RoadSection component; defaulting length to 10.");
        return 10f;
    }

    private void DespawnSection(ActiveSection section)
    {
        Destroy(section.go);
    }
}
