using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameState
{
    protected GameTimer timer;

    protected float stateTime;
    protected GameState nextState;
    protected bool IsStateEnd => timer.CurrentTime >= stateTime;

    public GameState()
    {
        timer = new GameTimer();
    }

    public abstract GameState Update();

    public void SetStateTime(float stateTime)
    {
        this.stateTime = stateTime;
    }

    public void SetNextState(GameState nextState)
    {
        this.nextState = nextState;
    }

    
}

public class EntranceState : GameState
{
    public override GameState Update()
    {
        timer.TimeClock();

        if(IsStateEnd)
        {
            timer.ResetTimer();
            
            return nextState;
        }

        return this;
    }
}

public class BeforeSelectingState : GameState
{
    public override GameState Update()
    {
        return this;
    }
}