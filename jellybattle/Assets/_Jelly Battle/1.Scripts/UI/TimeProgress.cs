using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeProgress : MonoBehaviour
{
    public Transform handle;
    public Transform goal;

    Vector3 startPos;
    Vector3 goalPos;

    public float dis;
    public float yOffset = .2f;

    public float progressRatio;

    private void Start()
    {
        startPos = handle.position;
        goalPos = goal.position;

        dis = startPos.x - goalPos.x;
    }

    private void Update()
    {
        
    }
    
    public void UpdateTimeProgress()
    {
        //progressRatio = GameManager.instance.curTime / GameManager.instance.selectingTime;

        //float xOffset = dis * progressRatio;

        //handle.position = new Vector3(goal.position.x + xOffset, goal.position.y + yOffset, goal.position.z);
    }

    public void UpdateTimeProgress(float currentTime, float stateTime)
    {
        progressRatio = 1f - (float)currentTime / stateTime;
        if (currentTime >= stateTime) progressRatio = 0;

        float xOffset = dis * progressRatio;

        handle.position = new Vector3(goal.position.x + xOffset, goal.position.y + yOffset, goal.position.z);
    }
}
