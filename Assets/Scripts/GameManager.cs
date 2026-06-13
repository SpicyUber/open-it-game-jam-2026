using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    public List<CarController> Cars;

    private GameState _state = GameState.Start;

    public UnityEvent<int> StartCountdownChanged;

    private Queue<CarController> _enemyQueue;
    void Start()
    {
        _enemyQueue = new();

        while (Cars.Count > 0)
        {
            int randIndex = UnityEngine.Random.Range(0, Cars.Count);
            _enemyQueue.Enqueue(Cars[randIndex]);
            Cars.RemoveAt(randIndex);
        }

        float s = 1f;
        foreach(CarController car in _enemyQueue)
        {
            car.SetT(s+=2f);
            car.Freeze();
        }

        StartCoroutine(StartGame());
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
        
    }

    private void TransitionTo() 
    { 
    }
    


}
