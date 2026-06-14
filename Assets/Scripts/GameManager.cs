using DG.Tweening;
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

    public GameObject SpikeFX, SmokeFX, FireFX, GumFX, OilFX;

    private GameState _state = GameState.Start;

    public UnityEvent<int> StartCountdownChanged;

    private int moveTurnCount = 2;
    private int _resolvedTurnCount = 0;
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

    IEnumerator ApplyEffectsRoutine(CarController player,CarController enemy,Card playerCard, Card enemyCard , CarController playerTarget, CarController enemyTarget) 
    {
        ShootAbilityFX(playerCard, playerTarget,player);

        yield return new WaitForSeconds(2.5f);

        ShootAbilityFX(enemyCard, enemyTarget,enemy);

        yield return new WaitForSeconds(2.5f);

        TryEndFight();
    }

    private void ShootAbilityFX(Card card, CarController target, CarController caster)
    {
        caster.Movement.ModelTransform.DOPunchScale(Vector3.one * 0.3f, 0.4f, 5, 0.5f);
        GameObject fx = GetEffectPrefab(card.Effect);
        if (fx == null) return;

        foreach (TargetLane lane in card.targetLanes)
        {
            Vector3 basePos = WaypointManager.Instance.GetPosition(target.GetT());

            Vector3 laneOffset = 6* ((int)lane - 1) * (WaypointManager.Instance.GetRotation(target.GetT()) * Vector3.right);
            Vector3 spawnPos = basePos + laneOffset;

            GameObject instance = Instantiate(fx, spawnPos, Quaternion.identity);

            var particles = instance.GetComponentsInChildren<ParticleSystem>();
            foreach (var ps in particles)
            {
                var main = ps.main;
                main.startColor = card.Color;
            }

            
            
                        GameObject railAnchor = new GameObject("FX_RailAnchor");
            Rail rail = railAnchor.AddComponent<Rail>();
            rail.SetT(target.GetT());

            instance.transform.SetParent(railAnchor.transform);
            instance.transform.localPosition = laneOffset; 

            Destroy(railAnchor, 2f);
        }
    }

    private GameObject GetEffectPrefab(EffectType effect)
    {
        return effect switch
        {
            EffectType.Spike => SpikeFX,
            EffectType.Smoke => SmokeFX,
            EffectType.Fire => FireFX,
            EffectType.Gum => GumFX,
            EffectType.Oil => OilFX,
            _ => null
        };
    }
    void UseCard(Card card)
    {
        Card playerCard = null;
        CarController playerTarget = null;
        Card enemyCard = null;
        CarController enemyTarget = null;

        for (int i = 0; i < Hand.Length; i++)
        {
            if (Hand[i] == card)
            {
              playerCard =  UseCardAbility(card, _enemyQueue.Peek(), Player);
                Hand[i] = null;
                Deck.Add(card);
            }
        }

        enemyCard = UseCardAbility(_enemyQueue.Peek().RandomAbility(), Player, _enemyQueue.Peek());
        if (enemyCard != null && enemyCard.cardType == CardType.Buff) { enemyTarget = _enemyQueue.Peek(); } else { enemyTarget = Player; }
        if (playerCard != null && playerCard.cardType == CardType.Buff) { playerTarget = Player; }else { playerTarget = _enemyQueue.Peek(); }

        _resolvedTurnCount++;
        LogTurnResourceState(_enemyQueue.Peek());

        PlayerSelectedCard = null;

        StartCoroutine(ApplyEffectsRoutine(Player, _enemyQueue.Peek(), playerCard,enemyCard,playerTarget, enemyTarget));

        
    }

    private void LogTurnResourceState(CarController enemy)
    {
        string enemyName = enemy != null ? enemy.gameObject.name : "No enemy";
        string enemyFuel = enemy != null ? enemy.Fuel.CurrentFuel.ToString("0") : "-";
        string enemyNitro = enemy != null ? enemy.Nitro.CurrentNitro.ToString("0") : "-";

        Debug.Log(
            $"END TURN {_resolvedTurnCount} | " +
            $"PLAYER ({Player.gameObject.name}) Fuel: {Player.Fuel.CurrentFuel:0}, Nitro: {Player.Nitro.CurrentNitro:0} | " +
            $"ENEMY ({enemyName}) Fuel: {enemyFuel}, Nitro: {enemyNitro}"
        );
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

    private Card UseCardAbility(Card card, CarController target, CarController caster)
    {
        if (card == null || target == null || caster == null) return null;

        Debug.Log($"USED {card.cardName} | Type: {card.cardType}");

        if (!caster.SpendNitro(card.nitroPoints))
        {
            Debug.Log(caster.gameObject.name + ": NOT ENOUGH NITRO!!!");
            return null;
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
            case CardType.WildCard:
                ApplyWildCard(card, target, caster);
                break;
        }

        return card;
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

    private void ApplyWildCard(Card card, CarController target, CarController caster)
    {
        WildCard wildCard = card as WildCard;
        if (wildCard == null) return;

        bool success = UnityEngine.Random.Range(0, 100) < wildCard.percentageToHappen;
        CarController affectedCar = success ? target : caster;
        int fuelDamage = success ? wildCard.opponentFuelDebuff : wildCard.playerFuelDebuff;
        int nitroDamage = success ? wildCard.opponentNitroDebuff : wildCard.playerNitroDebuff;

        affectedCar.AddFuel(-fuelDamage);
        affectedCar.AddNitro(-nitroDamage);

        Debug.Log(success
            ? $"WildCard success: {target.gameObject.name} loses {fuelDamage} fuel and {nitroDamage} nitro"
            : $"WildCard failed: {caster.gameObject.name} loses {fuelDamage} fuel and {nitroDamage} nitro");
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
        }

        _state = state;
    }
}
