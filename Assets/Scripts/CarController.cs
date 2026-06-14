using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarMovement))]
[RequireComponent(typeof(EffectPlayer))]
public class CarController : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;

    private CarMovement _carMovement;
    private EffectPlayer _effectPlayer;

    public PlayerFuel Fuel;
    public PlayerNitro Nitro;

    public AudioClip changeLaneSound;
    private AudioSource audioSource;

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

        audioSource = GetComponent<AudioSource>();
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
    
    public void MoveLeft() => TryChangeLane(Vector3Int.left, -1, "MOVE LEFT!");

    public void MoveRight() => TryChangeLane(Vector3Int.right, 1, "MOVE RIGHT!");

    private void TryChangeLane(Vector3Int direction, int alignmentDelta, string debugMessage)
    {
        if (_moving) return;

        int targetAlignment = _alignment + alignmentDelta;
        if (targetAlignment < -1 || targetAlignment > 1)
        {
            Stay();
            return;
        }

        Debug.Log(debugMessage);
        _moving = true;
        _alignment = targetAlignment;

        _carMovement.Move(_carMovement.CalculateMove(direction), 1f, () =>
        {
            _effectPlayer.PlayDustCloud();
            _moving = false;
        });

        if (changeLaneSound != null && audioSource != null)
            audioSource.PlayOneShot(changeLaneSound);
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
        foreach (var f in GetComponents<PlayerFuel>())
            f.ModifyFuel(-damage);
    }

    //returns true if nitro spent
    public bool SpendNitro(int nitro)
    {
        if (Nitro.CurrentNitro < nitro) return false;

        foreach (var n in GetComponents<PlayerNitro>())
            n.ModifyNitro(-nitro);

        return true;
    }

    public void Stay()
    {
        if (_moving) return;

        Debug.Log("STAY!");
    }

    public bool IsMoving => _moving;

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
        foreach (var n in GetComponents<PlayerNitro>())
            n.ModifyNitro(nitroMod);
    }

    internal void AddFuel(int fuelMod)
    {
        foreach (var f in GetComponents<PlayerFuel>())
            f.ModifyFuel(fuelMod);
    }

    internal void ExplodeYourself()
    {
        //TO DO EXPLODE ENEMY
        Vector3 enemyPos = this.transform.position;
        GameObject explosion = Instantiate(this.explosionPrefab, enemyPos, Quaternion.identity);
        Destroy(explosion, 3f);
        Destroy(this.gameObject);
    }
}
