using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Visual")]
    public Image cardImage;

    [Header("Card Data")]
    public TMP_Text cardName;
    public TMP_Text effectDescription;
    public TMP_Text nitroPoints;

    [Header("Events")]
    public UnityEvent<Card> onCardClicked;

    private Button button;
    private RectTransform rectTransform;

    private Card _card = null;

    public AudioClip hoverSound;
    private AudioSource audioSource;

    private Vector2 originalPos;

    void Awake()
    {
        button = GetComponent<Button>();
        rectTransform = GetComponentInChildren<RectTransform>();


        originalPos = rectTransform.anchoredPosition;

        audioSource = GetComponent<AudioSource>();
    }

    private void OnDisable()
    {
        rectTransform.DOKill();
    }

    private void OnEnable()
    {
        rectTransform.DOKill();
    }

    public void Init(Card card)
    {
        if (card == null)
        {
          
            return;
        }


        cardName.text = card.name;
        cardImage.sprite = card.cardImage;
        nitroPoints.text = $"{card.nitroPoints}";
        effectDescription.text = card.effectDescription;

        _card = card;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rectTransform.DOKill();
        rectTransform.DOAnchorPos(originalPos + new Vector2(0, 260f), 0.15f)
            .SetEase(Ease.OutQuad);
        if (hoverSound != null)
            audioSource.PlayOneShot(hoverSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rectTransform.DOKill();
        rectTransform.DOAnchorPos(originalPos, 0.15f)
            .SetEase(Ease.OutQuad);
    }

    public void SendCardInfo(){
        Debug.Log("TRYING TO SEND" + _card.cardName);
        GameManager.Instance.SelectCardPlayer(_card); }
}