using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuicideKeyCap : KeyCap
{
    public override void Use(Player player, Player[] enemies)
    {
        isActive = false;

        KeyBoard.instance.GetKeyCapCurrentWorldPositon(player.transform.position).GetComponent<Tile>().Attacked();
        player.SetKeyCapAttackedUI(player.transform.position.x, player.transform.position.y, true);

        player.Damaged(damage, 0);
        EffectManager.instance.Explode(player.transform.position);
        EffectManager.instance.ExplodeFeaderWithBomb(player.transform.position);

        foreach (Player enemy in enemies)
        {
            KeyBoard.instance.GetKeyCapCurrentWorldPositon(enemy.transform.position).GetComponent<Tile>().Attacked();
            player.SetKeyCapAttackedUI(enemy.transform.position.x, enemy.transform.position.y, true);

            enemy.Damaged(damage, 0);
            EffectManager.instance.Explode(enemy.transform.position);
            EffectManager.instance.ExplodeFeaderWithBomb(enemy.transform.position);
        }

        AudioManagerLocal.instance.Play("Bomb");
    }
}
