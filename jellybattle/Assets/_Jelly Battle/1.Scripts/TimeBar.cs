using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeBar : MonoBehaviour
{
    public Slider fill;

    public void UpdateTimeBar(float curTime, float maxTime)
    {
        fill.value = curTime / maxTime + 0.1f;
    }
}
