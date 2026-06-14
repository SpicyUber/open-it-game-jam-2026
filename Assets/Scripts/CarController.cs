using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarMovement))]
[RequireComponent(typeof(EffectPlayer))]
public class CarController : MonoBehaviour
{
    private CarMovement _carMovement;
    private EffectPlayer _effectPlayer;

    public PlayerFuel Fuel;
    public PlayerNitro Nitro;

    [SerializeField]
    Card[] enemyAbilites;

    public Card RandomAbility()
    {
        return enemyAbilites[UnityEngine.Random.Range(0, enemyAbilites.Length)];
    }

    public EffectPlayer EffectPlayer => _effectPlayer;

    private bool _moving = false;

    private int _alignment = 0;

    [SerializeField]
    GridDisplayLogic _gridLogic;

    private Rail _rail;

    public void Awake()
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
        WaypointManager.Instance.GetPosition(_rail.GetT()), 0.75f);
    }

    public void SetT(float t) => _rail.SetT(t);
    public void MoveLeft()
    {
        _moving = true;
        Debug.Log("MOVE LEFT!");
        if (_alignment < 0) return;
        _alignment--;
        _carMovement.Move(_carMovement.CalculateMove(Vector3Int.left), 1f, () => { _effectPlayer.PlayDustCloud(); GameManager.Instance.EndMoveTurn(); _moving = false; });
    }

    public void MoveRight()
    {
        _moving = true;
        Debug.Log("MOVE RIGHT!");
        if (_alignment > 0) return;
        _alignment++;
        _carMovement.Move(_carMovement.CalculateMove(Vector3Int.right), 1f, () => { _effectPlayer.PlayDustCloud(); GameManager.Instance.EndMoveTurn(); _moving = false; });
    }

    public bool IsHit(List<TargetLane> lanes)
    {
        bool found = false;

        foreach (var lane in lanes)
            if ((int)lane == _alignment)
                found = true;

        return found;

    }

    public void TakeDamage(int damage)
    {
        Fuel.ModifyFuel(-damage);
    }

    //returns true if nitro spent
    public bool SpendNitro(int nitro)
    {
        if (Nitro.CurrentNitro < nitro) return false;

        Nitro.ModifyNitro(-nitro);

        return true;
    }

    public void Stay() { _moving = true; Debug.Log("STAY!"); GameManager.Instance.EndMoveTurn(); _moving = false; }

    public void Freeze()
    {
        _rail.Freeze();
    }

    public void SpeedUp(int speedMult) => _rail.VisualSpeed = Rail.BaseVisualSpeed * speedMult;

    public void SpeedReset() => _rail.VisualSpeed = Rail.BaseVisualSpeed;

    public void UnFreeze()
    {
        _rail.UnFreeze();
    }

    public float GetT() => _rail.GetT();

    internal void AddNitro(int nitroMod)
    {
        Nitro.ModifyNitro(nitroMod);
    }

    internal void AddFuel(int fuelMod)
    {
        Fuel.ModifyFuel(fuelMod);
    }

    internal void ExplodeYourself()
    {
        //TO DO EXPLODE ENEMY
        Destroy(this.gameObject);
    }
}
