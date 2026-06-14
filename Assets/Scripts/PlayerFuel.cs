using UnityEngine;
using UnityEngine.Events;

public class PlayerFuel : MonoBehaviour
{
    private const float MaxFuel = 100f;

    private float _currentFuel;

    public UnityEvent OnDeath;
    public UnityEvent<int> OnChange;
    public float CurrentFuel => _currentFuel;

    public void Start() => SetFuel(MaxFuel);

    public void ModifyFuel(float delta)
    {
        SetFuel(_currentFuel + delta);
    }

    private void SetFuel(float value)
    {
        float previousFuel = _currentFuel;
        _currentFuel = Mathf.Clamp(value, 0f, MaxFuel);
        OnChange?.Invoke((int)_currentFuel);

        if (previousFuel > 0f && _currentFuel == 0f)
            OnDeath?.Invoke();
    }
}
