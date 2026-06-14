using UnityEngine;
using UnityEngine.Events;

public class PlayerNitro : MonoBehaviour
{
    private const float MaxNitro = 100f;

    private float _currentNitro;

    public UnityEvent OnDeath;
    public UnityEvent<int> OnChange;
    public float CurrentNitro => _currentNitro;

    public void Start() => SetNitro(MaxNitro);

    public void ModifyNitro(float delta)
    {
        SetNitro(_currentNitro + delta);
    }

    private void SetNitro(float value)
    {
        float previousNitro = _currentNitro;
        _currentNitro = Mathf.Clamp(value, 0f, MaxNitro);
        OnChange?.Invoke((int)_currentNitro);

        if (previousNitro > 0f && _currentNitro == 0f)
            OnDeath?.Invoke();
    }
}
