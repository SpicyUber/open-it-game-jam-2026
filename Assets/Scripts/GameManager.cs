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

    private Queue<CarController> _enemyQueue;
    private bool BehindNextEnemy => (_enemyQueue.Peek().GetT() - Player.GetT()) * (SpeedBiggerThanNextEnemy? 1 : -1) < 5f;

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

        Player.Freeze();
        Player.HideGrid();

        float s = 40f;
        foreach(CarController car in _enemyQueue)
        {
            car.SetT(s+=90f);
            car.Freeze();
            car.HideGrid();
        }

        StartCoroutine(StartGame());
    }

    void Update()
    {
        UpdateAccordingToState();
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

        TransitionTo(GameState.SpeedUp);
    }

    private void TransitionTo(GameState state) 
    {
        //transition out of original
        switch(_state)
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

        }

        //transition into new
        switch (state) 
        {
            case GameState.SpeedUp:
                Debug.Log("SPEED UP START");
                Player.SpeedUp(3);
                break;
        }

        _state = state;
    }
    


}
