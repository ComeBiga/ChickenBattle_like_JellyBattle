using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class BuffUI
{
    public string name;
    public GameObject UIobject;
    public TextMeshProUGUI textMesh;

    public void SetName(string _name)
    {
        name = _name;
    }

    public void SetObjectActive(bool value)
    {
        UIobject.SetActive(value);
    }

    public void SetText(string _text)
    {
        textMesh.text = _text;
    }
}
