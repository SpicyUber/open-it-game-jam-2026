using UnityEngine;
using UnityEngine.Events;

public class PlayerNitro : MonoBehaviour
{
    private float _currentFuel;

    public UnityEvent OnDeath;
    public UnityEvent<int> OnChange;
    public float CurrentNitro => _currentFuel;

    public void Start() => ModifyNitro(100f);
    public void ModifyNitro(float v)
    {
        int val = (int)v;

        _currentFuel = Mathf.Clamp(_currentFuel + val, 0, 100f);

        OnChange?.Invoke((int)_currentFuel);

        if (_currentFuel == 0) OnDeath?.Invoke();

    }
}
