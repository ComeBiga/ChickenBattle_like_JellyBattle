using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlideBar : MonoBehaviour
{
    public Slider fill;
    public TMPro.TextMeshProUGUI HP;

    public void UpdateFillBar(float current, float max)
    {
        fill.value = current / max;
        HP.text = current.ToString();
    }
}
