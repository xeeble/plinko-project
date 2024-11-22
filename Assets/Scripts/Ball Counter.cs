using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class BallCounter : MonoBehaviour
{
    public TextMeshProUGUI counter;
    public Spawner spawner;

    private void OnEnable()
    {
        EventsHandler.OnBallSpawned.AddListener(HandleBallSpawned);
    }

    private void Start()
    {
        UpdateBallCounter();
    }

    private void HandleBallSpawned(Ball arg0)
    {
        UpdateBallCounter();
    }

    public void UpdateBallCounter() { counter.text = spawner.ballsRemaining.ToString(); }

    private void OnDisable()
    {
        EventsHandler.OnBallSpawned.RemoveListener(HandleBallSpawned);
    }
}
