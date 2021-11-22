using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerKeyCap : KeyCap
{
    public override void Use(Player player, Player[] enemies)
    {
        isActive = false;

        player.Damaged(damage, 1);

        KeyBoard.instance.GetKeyCapCurrentWorldPositon(player.transform.position).GetComponent<Tile>().Attacked();
        player.SetKeyCapAttackedUI(player.transform.position.x, player.transform.position.y, true);


        AudioManagerLocal.instance.Play("Suicide");

    }
}
