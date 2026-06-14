using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName ="New Card", menuName ="Card")]
public class Card : ScriptableObject
{
    public Sprite cardImage;
    public string cardName;
    public CardType cardType; //att, def...
    public string effectDescription;
    public int nitroPoints; //koliko kosta da se aktivira
    public int damage; //koliko damage-a dilujemo protivnikovom fuel-u
    public int nitroBuff; // ZA (DE)BUFF koliko dodaje/oduzima nitra
    public int fuelBuff; // ZA (DE)BUFF koliko dodaje/oduzima fuel-a
    public List<TargetLane> targetLanes; // koje lejnove pogadja
    public EffectType Effect;
    public Color Color;
}
