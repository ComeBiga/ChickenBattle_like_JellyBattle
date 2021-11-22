using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public KeyCap keyCap;

    [Header("Mark")]
    public GameObject selected;
    public GameObject available;
    public GameObject attacked;

    bool isAttacked = false;

    private void Start()
    {
        selected = transform.Find("Selected").gameObject;
        available = transform.Find("Available").gameObject;
        attacked = transform.Find("Attacked").gameObject;
    }

    public void Select()
    {
        selected.SetActive(true);
    }

    public void UnSelect()
    {
        selected.SetActive(false);
    }

    public void Available()
    {
        available.SetActive(true);
    }

    public void UnAvailable()
    {
        available.SetActive(false);
    }

    public void Attacked()
    {
        if (attacked != null)
        {
            attacked.SetActive(true);
        }
    }

    public void UnAttacked()
    {
        if (attacked != null)
        {
            attacked.SetActive(false);
        }
    }
}
