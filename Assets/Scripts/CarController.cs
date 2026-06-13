using NUnit.Framework;
using UnityEngine;

[RequireComponent(typeof(CarMovement))]
[RequireComponent(typeof(EffectPlayer))]
public class CarController : MonoBehaviour
{
    private CarMovement _carMovement;
    private EffectPlayer _effectPlayer;
    private Rail _rail;

    [SerializeField]
    private BaseStatsSO _baseStats;

    public void Start()
    {
        _rail = GetComponentInParent<Rail>();
        _carMovement = GetComponent<CarMovement>();
        _effectPlayer = GetComponent<EffectPlayer>();
        MoveLeft();
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

    public void UnFreeze()
    {
        _rail.UnFreeze();
    }
}
