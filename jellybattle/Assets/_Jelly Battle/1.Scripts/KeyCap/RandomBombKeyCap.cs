using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBombKeyCap : KeyCap
{
    public override void Use(Player player, Player[] enemies)
    {
        isActive = false;

        int[] randomDamage = new int[] { 1, 10, 50 };

        string seed = Time.time.ToString();

        System.Random random = new System.Random(seed.GetHashCode());

        if (random.Next(4) == 3)
        {
            player.Damaged(30, 1);

            KeyBoard.instance.GetKeyCapCurrentWorldPositon(player.transform.position).GetComponent<Tile>().Attacked();
            player.SetKeyCapAttackedUI(player.transform.position.x, player.transform.position.y, true);

            EffectManager.instance.Explode(player.transform.position);
            EffectManager.instance.ExplodeFeaderWithBomb(player.transform.position);
        }
        else
        {
            foreach (Player enemy in enemies)
            {
                enemy.Damaged(randomDamage[random.Next(3)], 0);

                KeyBoard.instance.GetKeyCapCurrentWorldPositon(enemy.transform.position).GetComponent<Tile>().Attacked();
                player.SetKeyCapAttackedUI(enemy.transform.position.x, enemy.transform.position.y, true);

                EffectManager.instance.Explode(enemy.transform.position);
                EffectManager.instance.ExplodeFeaderWithBomb(enemy.transform.position);
            }

            player.anim.AttackAnimation();
        }

        
        AudioManagerLocal.instance.Play("Bomb");
    }
}
