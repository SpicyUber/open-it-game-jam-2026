using UnityEngine;
using UnityEngine.Events;

public class PlayerFuel : MonoBehaviour
{
    private float _currentFuel;

    public UnityEvent OnDeath;
    public UnityEvent<int> OnChange;
    public float CurrentFuel => _currentFuel;

    public void Start() => ModifyFuel(100);
    public void ModifyFuel(float v)
    {
        int val = (int)v;

        _currentFuel = Mathf.Clamp(_currentFuel + val,0,100f);

        OnChange?.Invoke((int)_currentFuel);

        if (_currentFuel == 0) OnDeath?.Invoke();

    }
}
