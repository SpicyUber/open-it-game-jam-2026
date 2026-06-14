using UnityEngine;

public class OnCardClick : MonoBehaviour
{
    public void OnCardClicked(CardDisplay card)
    {
        Debug.Log("Card clicked: " + card.cardName);
    }

}
