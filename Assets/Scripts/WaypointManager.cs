using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : Singleton<WaypointManager>
{
    public List<Transform> Points;
    public Vector3 GetPosition(float t)
    {
        float decimalLeftover = t - (int)t;
        int index = (int)t;
        index = index % Points.Count;
        int indexSecond = (index + 1) % Points.Count;

        Vector3 firstVector = Points[index].position;
        Vector3 secondVector = Points[indexSecond].position;

        return Vector3.Lerp(firstVector, secondVector, decimalLeftover);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.orange;

        foreach (Transform t in Points)
            Gizmos.DrawCube(t.position, Vector3.one / 10f);

        for (int i = 1; i < Points.Count; i++)
            Gizmos.DrawLine(Points[i - 1].position, Points[i].position);
    }
}
