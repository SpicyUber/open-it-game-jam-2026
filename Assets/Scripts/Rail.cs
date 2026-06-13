using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Rail : MonoBehaviour
{
    float _t = 0;
    const float BaseVisualSpeed = 0.2f;
    float _visualSpeed = BaseVisualSpeed;

    private void Update()
    {
        transform.position = WaypointManager.Instance?.GetPosition(_t) ?? transform.position;
        _t += _visualSpeed * Time.deltaTime;
    }




}
