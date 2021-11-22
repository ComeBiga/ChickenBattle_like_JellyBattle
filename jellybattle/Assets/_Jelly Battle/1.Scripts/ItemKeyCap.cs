using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemKeyCap : MonoBehaviour
{
    public Item item;

    public GameObject selected;

    public GameObject Available;

    private void Start()
    {
        selected.SetActive(false);
    }
}
