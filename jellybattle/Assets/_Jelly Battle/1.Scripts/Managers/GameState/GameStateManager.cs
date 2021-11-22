using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager
{
    GameState state;

    GameState entranceState;
    GameState beforeSelectingState;
    GameState selectingState;
    GameState beforeMovingState;

    public GameStateManager()
    {
        InitState();
    }

    private void InitState()
    {
        CreateState();
        SetState();

        state = entranceState;
    }

    private void CreateState()
    {
        entranceState = new EntranceState();
        beforeSelectingState = new BeforeSelectingState();
        selectingState = new BeforeSelectingState();
    }

    private void SetState()
    {
        entranceState.SetNextState(beforeSelectingState);
        entranceState.SetStateTime(GameStateTime.instance.entranceTime);

        beforeSelectingState.SetNextState(selectingState);

        selectingState.SetNextState(beforeMovingState);
        selectingState.SetStateTime(GameStateTime.instance.selectingTime);
    }

    public void Update()
    {
        state = state.Update();
    }
}
