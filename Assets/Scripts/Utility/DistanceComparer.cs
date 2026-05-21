using UnityEngine;
using System.Collections.Generic;

public class DistanceComparer<T> : IComparer<T> where T : Component
{
    private readonly Transform referenceOrigin;

    /// <summary>
    /// Initializes the generic distance comparer.
    /// </summary>
    /// <param name="origin">The central object to measure distances from.</param>
    public DistanceComparer(Transform origin)
    {
        referenceOrigin = origin;
    }

    public int Compare(T a, T b)
    {
        // Handle missing/destroyed unity objects safely
        if (a == null && b == null) return 0;
        if (a == null) return 1;  // Puts null objects at the end
        if (b == null) return -1;

        Vector3 originPosition = referenceOrigin.position;

        // T inherits from Component, allowing direct access to .transform
        float squaredDistanceToA = (a.transform.position - originPosition).sqrMagnitude;
        float squaredDistanceToB = (b.transform.position - originPosition).sqrMagnitude;

        return squaredDistanceToA.CompareTo(squaredDistanceToB);
    }
}
