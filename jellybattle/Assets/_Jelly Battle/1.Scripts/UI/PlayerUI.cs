using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class PlayerUI
{
    public int id;
    public string ownerName;

    public TextMeshProUGUI playerName;
    public Slider HPbar;
    public TextMeshProUGUI HpAmount;
    public List<BuffUI> buffUI;

    public Transform buffParent;
    public BuffSlot[] slots;

    public void Init()
    {
        slots = buffParent.GetComponentsInChildren<BuffSlot>();

        foreach(BuffSlot slot in slots)
        {
            slot.Init();
        }
    }

    public void SetPlayerName(string name)
    {
        playerName.text = name;
    }

    public BuffUI FindUI(string name)
    {
        return buffUI.Find(ui => ui.name == name);
    }

    public void SetActiveUI(string name, bool value)
    {
        var ui = FindUI(name);
        ui.SetObjectActive(value);
    }

    public void SetText(string name, string _text)
    {
        var ui = FindUI(name);
        ui.SetText(_text);
    }

    public void SetHPBar(int currentHP, int maxHP)
    {
        if (currentHP < 0) currentHP = 0;
        HPbar.value = (float)currentHP / maxHP;
        HpAmount.text = currentHP.ToString();
    }
}


