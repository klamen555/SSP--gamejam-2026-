using UnityEngine;

/// <summary>
/// Attach this to every road section prefab used by RoadSectionSpawner.
/// It reports the section's length along the road so the spawner can chain
/// sections back-to-back with no gaps or overlaps, regardless of speed.
/// </summary>
[DisallowMultipleComponent]
public class RoadSection : MonoBehaviour
{
    [Tooltip("Length of this section along the road's forward axis. Leave at 0 to auto-calculate from colliders/renderers.")]
    public float sectionLength = 0f;

    [Tooltip("Axis the road extends along (local), used only when auto-calculating length.")]
    public Vector3 sectionLengthAxis = Vector3.forward;

    private void Reset()
    {
        AutoCalculateLength();
    }

    /// <summary>Returns the section length, calculating it once from bounds if not set manually.</summary>
    public float GetLength()
    {
        if (sectionLength <= 0f) AutoCalculateLength();
        return sectionLength;
    }

    private void AutoCalculateLength()
    {
        Bounds bounds;

        if (TryGetComponent<Collider>(out var col))
        {
            bounds = col.bounds;
        }
        else if (TryGetComponent<Renderer>(out var rend))
        {
            bounds = rend.bounds;
        }
        else
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
            {
                Debug.LogWarning($"[{nameof(RoadSection)}] '{name}' has no Collider or Renderer to measure. Defaulting length to 10.");
                sectionLength = 10f;
                return;
            }

            bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }
        }

        Vector3 axis = sectionLengthAxis.normalized;
        sectionLength = Mathf.Abs(Vector3.Dot(bounds.size, axis));

        if (sectionLength <= 0.01f)
        {
            sectionLength = bounds.size.magnitude; // fallback if axis didn't line up with bounds
        }
    }
}