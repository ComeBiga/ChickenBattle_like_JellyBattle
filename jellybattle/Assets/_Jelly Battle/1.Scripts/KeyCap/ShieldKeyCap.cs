using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldKeyCap : KeyCap
{
    public override void Use(Player player, Player[] enemies)
    {
        isActive = false;

        count = 2;

        AudioManagerLocal.instance.Play("GetEm");
    }
}
