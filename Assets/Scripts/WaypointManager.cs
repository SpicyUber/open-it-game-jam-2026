using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : Singleton<WaypointManager>
{
    public List<Transform> Points;

    private readonly List<float> _segmentLengths = new();
    private float _totalLength;
    public Vector3 GetPosition(float t)
    {
        if (Points.Count == 0)
            return Vector3.zero;

        t %= _totalLength;

        float accumulated = 0f;

        for (int i = 0; i < Points.Count; i++)
        {
            float segmentLength = _segmentLengths[i];

            if (t <= accumulated + segmentLength)
            {
                float localT = (t - accumulated) / segmentLength;

                int next = (i + 1) % Points.Count;

                return Vector3.Lerp(
                    Points[i].position,
                    Points[next].position,
                    localT
                );
            }

            accumulated += segmentLength;
        }

        return Points[0].position;
    }
    public void RecalculateLengths()
    {
        _segmentLengths.Clear();
        _totalLength = 0;

        for (int i = 0; i < Points.Count; i++)
        {
            int next = (i + 1) % Points.Count;

            float length = Vector3.Distance(
                Points[i].position,
                Points[next].position);

            _segmentLengths.Add(length);
            _totalLength += length;
        }
    }
   
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.orange;

        foreach (Transform t in Points)
            Gizmos.DrawCube(t.position, Vector3.one / 10f);

        for (int i = 1; i < Points.Count; i++)
            Gizmos.DrawLine(Points[i - 1].position, Points[i].position);
    }

    public Quaternion GetRotation(float t)
    {
        return Quaternion.LookRotation((GetPosition(t + 0.125f)-GetPosition(t)).normalized);
    }
}
