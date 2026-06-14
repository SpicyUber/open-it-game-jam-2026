using DG.Tweening;
using UnityEngine;

public class ValueSliderChanger : MonoBehaviour
{
    [SerializeField]
    UnityEngine.UI.Slider Slider;

    private void Awake()
    {
        if (Slider == null)
            Slider = GetComponent<UnityEngine.UI.Slider>();

        if (Slider == null) return;

        Slider.minValue = 0f;
        Slider.maxValue = 100f;
    }

    public void ChangeValue(int value) 
    {
        if (Slider == null) return;

        Slider.DOKill();
        Slider.DOValue(value, 1f);
    }
}
