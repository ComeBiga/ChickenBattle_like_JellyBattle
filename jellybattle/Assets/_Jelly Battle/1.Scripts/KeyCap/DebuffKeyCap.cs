using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffKeyCap : KeyCap
{
    public override void Use(Player player, Player[] enemies)
    {
        isActive = false;

        foreach(Player enemy in enemies)
        {
            enemy.keyCaps.RemoveItems();
            enemy.UpdateItemSlotUI();
        }

        player.anim.AttackAnimation();
        AudioManagerLocal.instance.Play("Debuff");
    }
}
