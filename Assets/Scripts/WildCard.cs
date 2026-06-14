using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "WildCard", menuName = "WildCard")]
public class WildCard : Card
{
    public int percentageToHappen;
    public int opponentFuelDebuff;
    public int opponentNitroDebuff;
    public int playerFuelDebuff;
    public int playerNitroDebuff;
}
