using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    [Header("Visual")]
    public Image cardImage;

    [Header("Card Data")]
    public TMP_Text cardName;
    public TMP_Text effectDescription;
    public TMP_Text nitroPoints;

    [Header("Events")]
    public UnityEvent<CardDisplay> onCardClicked;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        onCardClicked?.Invoke(this);  // šalje celu karticu
    }

}
