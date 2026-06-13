using DG.Tweening;
using System;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    [SerializeField]
    private Grid _grid;
    [SerializeField]
    private Transform positionTransform, modelTransform;

    public Vector3 CalculateMove(Vector3Int dir) 
    {
        return _grid.CellToWorld(_grid.WorldToCell(transform.position) + dir); 
    }


    private void InitStats(BaseStatsSO stats)
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(positionTransform.position, 1);
        Gizmos.DrawSphere(CalculateMove(Vector3Int.right),1);
        Gizmos.DrawSphere(CalculateMove(Vector3Int.left),1);

    }

    public void Move( Vector3 endPos ,float duration, Action onComplete = null, float tiltAngle = 45)
    {
        Vector3 startPos = positionTransform.position;

        Vector3 direction = (endPos-startPos).normalized;

        float angle = direction.x > 0 ? -tiltAngle : tiltAngle;


        Sequence sequence = DOTween.Sequence();

        sequence.Append(
            modelTransform.DORotate(
                new Vector3(0, 0, angle),
                duration * 0.2f
            )
        );

        sequence.Join(
            positionTransform.DOLocalMove(transform.InverseTransformPoint(endPos), duration)
        );

        sequence.Append(
            modelTransform.DORotate(
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
