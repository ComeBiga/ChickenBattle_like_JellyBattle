using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class BuffSlot : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI count;

    KeyCap buff;

    public void Init()
    {
        icon.enabled = false;
        count.enabled = false;
    }
    
    public void AddBuff(int iconImageCode, string _count)
    {
        //buff = newBuff;

        icon.sprite = UIManager.instance.buffImages[iconImageCode];
        count.text = _count;
        icon.enabled = true;
        count.enabled = true;
    }

    public void ClearSlot()
    {
        buff = null;

        icon.sprite = null;
        count.text = null;
        icon.enabled = false;
        count.enabled = false;
    }

    public void UpdateSlot()
    {
        count.text = buff.count.ToString();
    }
}
