using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceKeyCap : KeyCap
{
    public override void Use(Player player, Player[] enemies)
    {
        isActive = false;

        foreach(Player enemy in enemies)
        {
            enemy.anim.IcedAnimation();
            KeyBoard.instance.RemoveKeyCap(enemy.transform.position);
        }

        player.anim.AttackAnimation();
        //_player.anim.AttackAnimation();

        AudioManagerLocal.instance.Play("Ice");

    }
}
