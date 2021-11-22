using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverKeyCap : KeyCap
{
    public override void Use(Player player, Player[] enemies)
    {
        isActive = false;

        player.Recover(recover);

        AudioManagerLocal.instance.Play("Heal");
        EffectManager.instance.HealEffect(player.transform.position, recover);
    }
}
