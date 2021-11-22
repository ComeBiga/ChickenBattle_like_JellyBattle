using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleJumpKeyCap : KeyCap
{

    public override void Use(Player player, Player[] enemies)
    {
        isActive = false;

        count = 5;

        AudioManagerLocal.instance.Play("GetEm");

    }
}
