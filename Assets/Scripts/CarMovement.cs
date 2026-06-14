using DG.Tweening;
using System;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    [SerializeField]
    private Grid _grid;

    [SerializeField]
    public Transform PositionTransform, ModelTransform;

    public Vector3 CalculateMove(Vector3Int dir) 
    {
        return _grid.CellToWorld(_grid.WorldToCell(PositionTransform.position) + dir); 
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(PositionTransform.position, 1);
        Gizmos.DrawSphere(CalculateMove(Vector3Int.right),1);
        Gizmos.DrawSphere(CalculateMove(Vector3Int.left),1);

    }

    public void Move( Vector3 endPos ,float duration, Action onComplete = null, float tiltAngle = 45)
    {
        PositionTransform.DOKill();
        ModelTransform.DOKill();
        Vector3 startPos = PositionTransform.position;

        Vector3 direction = transform.InverseTransformDirection(endPos - startPos).normalized;

        float angle = direction.x > 0 ? -tiltAngle : tiltAngle;


        Sequence sequence = DOTween.Sequence();

        sequence.Append(
            ModelTransform.DOLocalRotate(
                new Vector3(0, 0, angle),
                duration * 0.2f
            )
        );

        sequence.Join(
            PositionTransform.DOLocalMove(transform.InverseTransformPoint(endPos), duration)
        );

        sequence.Append(
            ModelTransform.DOLocalRotate(
                Vector3.zero,
                duration * 0.2f
            )
            .SetEase(Ease.OutBounce) 
        );

        sequence.OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }
}
