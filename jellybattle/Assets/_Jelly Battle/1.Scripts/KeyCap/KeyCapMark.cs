using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCapMark : MonoBehaviour
{
    public GameObject selected;
    public GameObject available;

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
}
