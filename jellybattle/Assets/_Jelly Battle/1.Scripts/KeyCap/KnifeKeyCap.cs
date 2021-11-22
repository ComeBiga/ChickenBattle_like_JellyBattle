using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeKeyCap : KeyCap
{
    public override void Use(Player player, Player[] enemies)
    {
        isActive = false;

        count = 3;
        Attack(player, enemies);
    }

    private void Attack(Player player, Player[] enemies)
    {
        player.anim.AttackBoneAnimation();

        Vector3 playerPos = player.transform.position;

        foreach(Player enemy in enemies)
        {
            Vector3 enemyPos = enemy.transform.position;

            if (enemyPos.x >= playerPos.x - range && enemyPos.x <= playerPos.x + range &&
                    enemyPos.y >= playerPos.y - range && enemyPos.y <= playerPos.y + range)
            {
                enemy.Damaged(damage, 0);
            }
        }

        for (int x = (int)playerPos.x - range; x <= playerPos.x + range; x++)
        {
            for (int y = (int)playerPos.y - range; y <= playerPos.y + range; y++)
            {
                if (x != playerPos.x || y != playerPos.y)
                {
                    if (KeyBoard.instance.IsKeyBoard(new Vector3(x, y)))
                    {
                        KeyBoard.instance.GetKeyCapCurrentWorldPositon(new Vector3(x, y)).GetComponent<Tile>().Attacked();
                        player.SetKeyCapAttackedUI(x, y, true);
                    }
                }

            }
        }
    }

    public override void UseAsTool(Player player, Player[] enemies)
    {
        Attack(player, enemies);
    }
}
