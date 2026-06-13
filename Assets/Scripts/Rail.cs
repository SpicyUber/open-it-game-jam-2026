using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Rail : MonoBehaviour
{
    float _t = 0;
    bool _stopped = false;
    const float BaseVisualSpeed = 0.2f;
    float _visualSpeed = BaseVisualSpeed;

    private void Update()
    {
        transform.position = WaypointManager.Instance?.GetPosition(_t) ?? transform.position;

        if (_stopped) return;
        _t += _visualSpeed * Time.deltaTime;
    }

    public void SetT(float t) => _t = t;

    public void Freeze() => _stopped = true;
    public void UnFreeze() => _stopped = false;
}
