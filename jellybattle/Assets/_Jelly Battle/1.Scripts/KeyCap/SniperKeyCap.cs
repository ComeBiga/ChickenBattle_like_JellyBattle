using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperKeyCap : KeyCap
{
    public override void Use(Player player, Player[] enemies)
    {
        isActive = false;

        Vector3 playerPos = player.transform.position;

        foreach(Player enemy in enemies)
        {
            if (KeyBoard.instance.PositionWorldToKeyboard(enemy.transform.position).y
                == KeyBoard.instance.PositionWorldToKeyboard(player.transform.position).y)
            {
                enemy.Damaged(damage, 0);
                EffectManager.instance.ExplodeFeader(enemy.transform.position);
            }
        }

        GameObject[] allKeyCaps = KeyBoard.instance.GetKeyCapAll();

        foreach(GameObject keycap in allKeyCaps)
        {
            if(keycap.transform.position.x == playerPos.x)
            {
                keycap.GetComponent<Tile>().Attacked();
                player.SetKeyCapAttackedUI(keycap.transform.position.x, keycap.transform.position.y, true);
            }
        }

        player.anim.AttackSniperAnimation();
        AudioManagerLocal.instance.Play("Sniper");
    }
}
