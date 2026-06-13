using NUnit.Framework;
using UnityEngine;

[RequireComponent(typeof(CarMovement))]
[RequireComponent(typeof(EffectPlayer))]
public class CarController : MonoBehaviour
{
    private CarMovement _carMovement;
    private EffectPlayer _effectPlayer;

    public void Start()
    {
        _carMovement = GetComponent<CarMovement>();
        _effectPlayer = GetComponent<EffectPlayer>();
        MoveLeft();
    }
    public void MoveLeft()
    {
        _carMovement.Move(_carMovement.CalculateMove(Vector3Int.left), 1f, () => { _effectPlayer.PlayDustCloud(); });
    }

    public void MoveRight()
    {
        _carMovement.Move(_carMovement.CalculateMove(Vector3Int.right), 1f, () => { _effectPlayer.PlayDustCloud(); });
    }
}
