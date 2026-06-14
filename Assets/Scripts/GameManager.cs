using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public List<CarController> Cars;
    public List<Card> Deck;
    public Card[] Hand;
    public CardDisplay[] CardDisplays;
    public CarController Player;

    private GameState _state = GameState.Start;

    public UnityEvent<int> StartCountdownChanged;

    private int moveTurnCount = 2;
    public Card PlayerSelectedCard = null;

    public GameObject CardUI, MoveUI;

    private Queue<CarController> _enemyQueue;
    private bool BehindNextEnemy => (_enemyQueue.Peek().GetT() - Player.GetT()) * (SpeedBiggerThanNextEnemy ? 1 : -1) < 5f;
    private bool SpeedBiggerThanNextEnemy => true;

    void Start()
    {
        _enemyQueue = new();

        while (Cars.Count > 0)
        {
            int randIndex = UnityEngine.Random.Range(0, Cars.Count);
            _enemyQueue.Enqueue(Cars[randIndex]);
            Cars.RemoveAt(randIndex);
        }

        MoveUI.SetActive(false);
        CardUI.SetActive(false);

        Player.Freeze();
        Player.HideGrid();
        Player.EffectPlayer.ToggleDustTrailOff();
        WaypointManager.Instance.RecalculateLengths();

        float s = 40f;
        foreach (CarController car in _enemyQueue)
        {
            car.SetT(s += 90f);
            car.Freeze();
            car.HideGrid();
            car.EffectPlayer.ToggleDustTrailOff();
        }

        StartCoroutine(StartGame());
    }

    void LateUpdate()
    {
        UpdateAccordingToState();
    }

    void RefillCards()
    {
        if (Hand == null)
            Hand = new Card[4];

        for (int i = 0; i < 4; i++)
        {
            if (Hand[i] == null)
            {
                var card = Deck[0];
                Deck.RemoveAt(0);
                Hand[i] = card;
            }
        }

        for (int i = 0; i < CardDisplays.Length; i++)
        {
            CardDisplays[i].Init(Hand[i]);
        }
    }

    void UseCard(Card card)
    {
        for (int i = 0; i < Hand.Length; i++)
        {
            if (Hand[i] == card)
            {
                UseCardAbility(card, _enemyQueue.Peek(), Player);
                Hand[i] = null;
                Deck.Add(card);
            }
        }

        UseCardAbility(_enemyQueue.Peek().RandomAbility(), Player, _enemyQueue.Peek());

        PlayerSelectedCard = null;

        TryEndFight();
    }

    private void TryEndFight()
    {
        if (_enemyQueue.Count == 0)
        {
            TransitionTo(GameState.Victory);
            return;
        }

        if (Player.Fuel.CurrentFuel == 0)
        {
            SceneManager.LoadScene(0);
            return;
        }

        if (_enemyQueue.Peek().Fuel.CurrentFuel == 0)
        {
            var defeatedEnemy = _enemyQueue.Dequeue();
            defeatedEnemy.ExplodeYourself();

            if (_enemyQueue.Count > 0)
                TransitionTo(GameState.SpeedUp);
            else
                TransitionTo(GameState.Victory);
        }
        else
        {
            TransitionTo(GameState.PickCard);
        }
    }

    private void UseCardAbility(Card card, CarController target, CarController caster)
    {
        Debug.Log($"USED {card.cardName} | Type: {card.cardType}");

        if (!caster.SpendNitro(card.nitroPoints))
        {
            Debug.Log("NOT ENOUGH NITRO!!!");
            return;
        }

        switch (card.cardType)
        {
            case CardType.Attack:
                ApplyAttack(card, target);
                break;
            case CardType.Defence:
                ApplyDefence(card, target);
                break;
            case CardType.Buff:
                ApplyBuff(card, caster, isDebuff: false);
                break;
            case CardType.Debuff:
                ApplyBuff(card, target, isDebuff: true);
                break;
        }
    }

    private void ApplyAttack(Card card, CarController target)
    {
        Debug.Log($"Attacking {target.gameObject.name} for {card.damage} damage");

        if (target.IsHit(card.targetLanes))
        {
            target.TakeDamage(card.damage);
            Debug.Log($"Attacking hit");
        }
        else
        {
            Debug.Log($"Attacking missed");
        }
    }

    private void ApplyDefence(Card card, CarController target)
    {
    }

    private void ApplyBuff(Card card, CarController target, bool isDebuff)
    {
        int nitroMod = isDebuff ? -card.nitroBuff : card.nitroBuff;
        int fuelMod = isDebuff ? -card.fuelBuff : card.fuelBuff;

        Debug.Log($"Defending — fuel buff: {card.fuelBuff} and nitro buff {card.nitroBuff}");

        target.AddNitro(nitroMod);
        target.AddFuel(fuelMod);
    }

    public void SelectCardPlayer(Card card)
    {
        if (_state != GameState.PickCard) return;
        if (card == null) return;

        PlayerSelectedCard = card;
        TransitionTo(GameState.PickMove);
    }

    public void PlayerMoveLeft() => ExecuteMove(() => Player.MoveLeft());
public void PlayerMoveRight() => ExecuteMove(() => Player.MoveRight());
public void PlayerStay() => ExecuteMove(() => Player.Stay());

private void ExecuteMove(Action playerMove)
{
    if (_state != GameState.PickMove) return;

    playerMove();

    var enemy = _enemyQueue.Peek();
    float r = UnityEngine.Random.value;
    if (r < 0.33f) enemy.MoveLeft();
    else if (r < 0.66f) enemy.MoveRight();
    else enemy.Stay();

    StartCoroutine(WaitForMovesThenResolve());
}

private IEnumerator WaitForMovesThenResolve()
{
    yield return new WaitUntil(() => !Player.IsMoving && !_enemyQueue.Peek().IsMoving);
    TransitionTo(GameState.TurnResult);
}

    private void UpdateAccordingToState()
    {
        switch (_state)
        {
            case GameState.Start:
                break;
            case GameState.SpeedUp:
                if (BehindNextEnemy)
                {
                    _enemyQueue.Peek().ShowGrid();
                    Player.ShowGrid();
                    TransitionTo(GameState.PickCard);
                }
                break;
            case GameState.PickCard:
                break;
            case GameState.PickMove:
                break;
        }
    }

    System.Collections.IEnumerator StartGame()
    {
        int countdown = 3;

        StartCountdownChanged?.Invoke(countdown);
        yield return new WaitForSeconds(1);
        StartCountdownChanged?.Invoke(--countdown);
        yield return new WaitForSeconds(1);
        StartCountdownChanged?.Invoke(--countdown);
        yield return new WaitForSeconds(1);
        StartCountdownChanged?.Invoke(--countdown);

        Player.EffectPlayer.ToggleDustTrailOn();

        foreach (CarController car in _enemyQueue)
        {
            car.EffectPlayer.ToggleDustTrailOn();
        }

        TransitionTo(GameState.SpeedUp);
    }

    private void TransitionTo(GameState state)
    {
        // transition out of current state
        switch (_state)
        {
            case GameState.Start:
                foreach (CarController car in _enemyQueue)
                {
                    car.UnFreeze();
                }
                Player.UnFreeze();
                CameraLogic.Instance.TransitionToAbove();
                break;
            case GameState.SpeedUp:
                Debug.Log("SPEED UP END");
                Player.SpeedReset();
                break;
            case GameState.PickCard:
                CardUI.SetActive(false);
                break;
            case GameState.PickMove:
                MoveUI.SetActive(false);
                break;
        }

        // transition into new state
        switch (state)
        {
            case GameState.SpeedUp:
                Debug.Log("SPEED UP START");
                Player.SpeedUp(3);
                break;
            case GameState.PickCard:
                CardUI.SetActive(true);
                RefillCards();
                break;
            case GameState.PickMove:
                moveTurnCount = 2;
                MoveUI.SetActive(true);
                break;
            case GameState.TurnResult:
                UseCard(PlayerSelectedCard);
                return;
                break;
        }

        _state = state;
    }
}