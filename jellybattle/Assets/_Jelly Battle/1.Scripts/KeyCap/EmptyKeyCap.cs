using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyKeyCap : KeyCap
{
    public override void Use(Player player, Player[] enemies)
    {
        isActive = false;
    }
}
