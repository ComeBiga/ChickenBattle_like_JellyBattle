using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject keycap;

    private void Start()
    {
        Instantiate(keycap, transform);
    }
}
