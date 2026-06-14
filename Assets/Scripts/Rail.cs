using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Rail : MonoBehaviour
{
    float _t = 0;
    bool _stopped = false;
    public const float BaseVisualSpeed = 15f;
    public float VisualSpeed = BaseVisualSpeed;

    private void Update()
    {
        transform.position = WaypointManager.Instance?.GetPosition(_t) ?? transform.position;

        if (_stopped) return;
        _t += VisualSpeed * Time.deltaTime;
    }

    private void Start()
    {
        VisualSpeed = BaseVisualSpeed;
    }

    public void SetT(float t) => _t = t;

    public void Freeze() => _stopped = true;
    public void UnFreeze() => _stopped = false;

    public float GetT()
    {
        return _t;
    }
}
