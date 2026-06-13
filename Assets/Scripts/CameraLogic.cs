using DG.Tweening;
using System.Collections;
using UnityEngine;

public class CameraLogic : Singleton<CameraLogic>
{
    public Transform Above, Behind;
    private Transform _target;

    private void Start()
    {
        transform.position = Behind.position;
        transform.rotation = Behind.rotation;

    }

    public void TransitionToAbove() => TransitionTo(Above);

    public void TransitionToBehind() => TransitionTo(Behind);
    public void TransitionTo(Transform target, float t = 4f)
    {
        StartCoroutine(TransitionToRoutine(target, t));
    }

    IEnumerator TransitionToRoutine(Transform target, float t)
    {
        float timer = 0f;
        _target = null;
        Quaternion startRotation = Camera.main.transform.rotation;
        Vector3 startPosition = Camera.main.transform.position;

        while (timer < t)
        {
            transform.rotation =
                Quaternion.Lerp(startRotation, target.rotation, timer / t);
            transform.position = Vector3.Lerp(startPosition, target.position, timer / t);
            yield return null;
            transform.rotation = target.rotation;
            transform.position = target.position;
            timer += Time.deltaTime;
        }
        _target = target;
    }

    private void LateUpdate()
    {
        if (_target == null) return;
        Camera.main.transform.position = _target.position;
        Camera.main.transform.rotation = _target.rotation;
    }
}
