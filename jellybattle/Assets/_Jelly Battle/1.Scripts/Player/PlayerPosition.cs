using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerPosition
{
    Vector2 transformPosition;
    [SerializeField] Vector2 current;
    [SerializeField] Vector2 next;

    public PlayerPosition(Vector2 tPosition)
    {
        transformPosition = tPosition;
    }

    public void SetCurrentPosition(float x, float y)
    {
        current = new Vector2(x, y);
    }

    public void SetCurrentPosition(Vector2 position)
    {
        current = position;
    }

    public void SetNextPosition(float x, float y)
    {
        next = new Vector2(x, y);
    }

    public void SetCurrentToNext()
    {
        current = next;
    }
}
