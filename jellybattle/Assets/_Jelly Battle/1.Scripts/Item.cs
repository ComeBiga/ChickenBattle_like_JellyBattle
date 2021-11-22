using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public new string name;
    public int id;

    public Animator animator;

    public abstract void Use();
}
