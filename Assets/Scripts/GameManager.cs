using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    public List<CarController> Cars;

    public CarController Player;
    private GameState _state = GameState.Start;

    public UnityEvent<int> StartCountdownChanged;

    private int moveTurnCount = 2;

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

    void Update()
    {
        UpdateAccordingToState();
    }

    public void EndMoveTurn()
    {
        if (_state != GameState.PickMove) return;

        if (moveTurnCount == 2)
        {
            CardUI.SetActive(false);
            var enemy = _enemyQueue.Peek();
            float r = UnityEngine.Random.value;

            if (r < 0.33f)
            {
                enemy.MoveLeft();
            }
            else if (r < 0.66f)
            {
                enemy.MoveRight();
            }
            else
            {
                enemy.Stay();
            }
        }
        else
        {
            TransitionTo(GameState.TurnResult);
        }

        moveTurnCount--;
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
                TransitionTo(GameState.PickMove);
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
        //transition out of original
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

        //transition into new
        switch (state)
        {
            case GameState.SpeedUp:
                Debug.Log("SPEED UP START");
                Player.SpeedUp(3);
                break;
            case GameState.PickCard:
                CardUI.SetActive(true);
                
                
                break;
            case GameState.PickMove:
                moveTurnCount = 2;
                MoveUI.SetActive(true);
                break;
        }

        _state = state;
    }



}
