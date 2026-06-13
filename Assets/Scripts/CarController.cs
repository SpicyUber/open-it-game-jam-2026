using NUnit.Framework;
using UnityEngine;

[RequireComponent(typeof(CarMovement))]
[RequireComponent(typeof(EffectPlayer))]
public class CarController : MonoBehaviour
{
    private CarMovement _carMovement;
    private EffectPlayer _effectPlayer;

    [SerializeField]
    GridDisplayLogic _gridLogic;

    private Rail _rail;

    [SerializeField]
    private BaseStatsSO _baseStats;

    public void Start()
    {
        _rail = GetComponentInParent<Rail>();
        _carMovement = GetComponent<CarMovement>();
        _effectPlayer = GetComponent<EffectPlayer>();
       // MoveLeft();
    }

    public void HideGrid() => _gridLogic.Hide();

    public void ShowGrid() => _gridLogic.Show();

    public void Update()
    {
        

        Quaternion targetRot = WaypointManager.Instance.GetRotation(_rail.GetT());

        Quaternion lerpRot = Quaternion.Slerp(
            _rail.transform.rotation,
            targetRot,
            10f * Time.deltaTime
        );

        _rail.transform.rotation = lerpRot;
        _carMovement.PositionTransform.rotation = lerpRot;
    }

    private void OnDrawGizmos()
    {
        if (WaypointManager.Instance == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(
        WaypointManager.Instance.GetPosition(_rail.GetT()),0.75f);
    }

    public void SetT(float t) => _rail.SetT(t);
    public void MoveLeft()
    {
        _carMovement.Move(_carMovement.CalculateMove(Vector3Int.left), 1f, () => { _effectPlayer.PlayDustCloud(); });
    }

    public void MoveRight()
    {
        _carMovement.Move(_carMovement.CalculateMove(Vector3Int.right), 1f, () => { _effectPlayer.PlayDustCloud(); });
    }

    public void Freeze()
    {
        _rail.Freeze();
    }

    public void SpeedUp(int speedMult) => _rail.VisualSpeed = Rail.BaseVisualSpeed *  speedMult;

    public void SpeedReset() => _rail.VisualSpeed = Rail.BaseVisualSpeed;

    public void UnFreeze()
    {
        _rail.UnFreeze();
    }

    public float GetT() => _rail.GetT();
}
