using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ValueSliderChanger : MonoBehaviour
{
    [SerializeField]
    UnityEngine.UI.Slider Slider;
    public void ChangeValue(int value) 
    {
        Slider.DOKill();
        Slider.DOValue(value, 1f);
    }
}
