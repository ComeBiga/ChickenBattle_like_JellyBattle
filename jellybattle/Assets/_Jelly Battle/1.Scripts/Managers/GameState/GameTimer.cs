using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer
{
    private float currentTime;
    public float CurrentTime { get { return currentTime; } }

    public GameTimer() { ResetTimer(); }
    public void ResetTimer() { currentTime = 0; }
    public void TimeClock() { currentTime += Time.deltaTime; }
}
