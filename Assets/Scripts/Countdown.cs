using TMPro;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    TextMeshProUGUI _tmp;
    void Start()
    {
        _tmp = GetComponent<TextMeshProUGUI>();
    }

    public void SetCountdown(int number)
    {
        _tmp.text = number.ToString();
        if (number == 0) { _tmp.text = ""; }
    }
}
