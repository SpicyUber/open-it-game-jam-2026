using UnityEngine;

[CreateAssetMenu(fileName = "BaseStatsSO", menuName = "Scriptable Objects/BaseStatsSO")]
public class BaseStatsSO : ScriptableObject
{
    public int MaxFuel { get; set; }
    public int Speed { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
}
