using TMPro;
using UnityEngine;

public class TextChangeLogicThing : MonoBehaviour
{
    public TextMeshProUGUI TextMeshProUGUI;

    public void UpdateNitroCount(int nitro) => TextMeshProUGUI.text = "" + nitro;

}
