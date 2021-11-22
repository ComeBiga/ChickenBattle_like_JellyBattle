using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomberKeyCap : KeyCap
{
    public int bombTime = 10;
    [Range(0, 100)]
    public float bombPercent = 70f;

    public override void Use(Player player, Player[] enemies)
    {
        isActive = false;

        string seed = (Time.time + Random.value).ToString();
        System.Random random = new System.Random(seed.GetHashCode());

        Vector3[] randomPos = new Vector3[bombTime];
        int randomIndex = 0;

        for(int x = 0; x < KeyBoard.instance.width; x++)
        {
            for(int y = 0; y < KeyBoard.instance.height; y++)
            {
                bool choice = (random.Next(0, 100) < bombPercent) ? true : false;

                if(choice)
                {
                    Debug.Log("RandomIndex : " + randomIndex);
                    randomPos[randomIndex] = KeyBoard.instance.PositionKeyboardToWorld(new Vector3(x + 1, y + 1));
                    randomIndex++;
                }

                if (randomIndex >= bombTime) break;
            }
            if (randomIndex >= bombTime) break;
        }

        foreach(Player enemy in enemies)
        {
            foreach(Vector3 bombPos in randomPos)
            {
                KeyBoard.instance.GetKeyCapCurrentWorldPositon(bombPos).GetComponent<Tile>().Attacked();
                player.SetKeyCapAttackedUI(bombPos.x, bombPos.y, true);

                EffectManager.instance.Explode(bombPos);

                if (bombPos == enemy.transform.position)
                {
                    enemy.Damaged(damage, 1);
                    EffectManager.instance.ExplodeFeaderWithBomb(enemy.transform.position);
                }
            }
        }

        player.anim.AttackAnimation();
        AudioManagerLocal.instance.Play("Bomb");
    }

}
