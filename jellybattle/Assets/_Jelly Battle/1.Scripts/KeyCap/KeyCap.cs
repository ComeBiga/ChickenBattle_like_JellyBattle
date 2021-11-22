using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class KeyCap : MonoBehaviour
{
    public bool isActive = true;

    public new string tag;
    public int code;
    public int imageCode;

    public int damage;
    public int recover;
    public int range;
    public int amount;
    public int count;

    public KeyCapType type;
    public UseTime UseTime;
    public ControlTurn controlTurn;

    public abstract void Use(Player player, Player[] enemies);
    public virtual void UseAsTool(Player player, Player[] enemies) { }

    /// <summary>
    /// count를 하나 감소시키고 count가 0이면 false를 반환, 아니면 true
    /// </summary>
    /// <returns></returns>
    public string CountDown()
    {
        if (count == 0)
            Debug.LogError("Count is already 0");

        count--;

        return tag;
    }

    protected void Damage(Player _player, int _damage, int type)
    {
        _player.Damaged(_damage, type);
    }

    protected void ExplodeEffect()
    {

    }
}

public enum KeyCapType { Normal, Buff, Tool, FalseTool }
public enum UseTime { Default, BeforeBattleStart, BeforeTurnStart }
public enum ControlTurn { Default, Steal, Change }
