using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class KeyCapManager : MonoBehaviour
{
    public List<KeyCap> keycaps;

    public KeyCap Find(int code)
    {
        KeyCap cap = keycaps.Find(keycap => keycap.code == code);

        return cap;
    }

    public void Use(int code)
    {
        //Find(code).Use();
    }
}
