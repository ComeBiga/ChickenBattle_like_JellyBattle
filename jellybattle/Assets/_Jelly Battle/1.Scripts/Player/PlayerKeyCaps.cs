using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;

public class PlayerKeyCaps : MonoBehaviour
{
    PhotonView photonView = new PhotonView();

    private KeyCap current;
    public KeyCap Current { get { return current; } }
    public Queue<KeyCap> keepCurrent = new Queue<KeyCap>();

    //public List<KeyCap> buff = new List<KeyCap>();
    //public void Setbuff() { if(current.type == KeyCapType.Buff) buff.Add(current); }
    //public Queue<KeyCap> tool = new Queue<KeyCap>();
    //public void SetTool() { if (tool.Count == 0 && current.type == KeyCapType.Tool) tool.Enqueue(current); }
    public bool HasTool => items.Any(item => item.type == KeyCapType.Tool);
    public bool Isbuffing(string tag) { return items.Any(keycap => keycap.tag == tag); }
    //public KeyCap GetBuffKeyCap(string tag) { return buff.Find(keycap => keycap.tag == tag); }

    public List<KeyCap> items = new List<KeyCap>();
    public void AddItem()
    {
        if(current.type == KeyCapType.Tool)
        {
            if (items.Any(item => item.type == KeyCapType.Tool)) return;
        }

        if(current.tag == "TripleJump")
        {
            GetComponent<PlayerSelectKeyCap>().moveRange = 3;
            photonView = PhotonView.Get(this);
            photonView.RPC("SetMoveRangeRPC", RpcTarget.Others, 3);
        }

        items.Add(current);
    }

    [PunRPC]
    private void SetMoveRangeRPC(int moveRange)
    {
        GetComponent<PlayerSelectKeyCap>().moveRange = moveRange;
    }

    public void CountDownBuffItem()
    {
        var buffItems = items.FindAll(item => item.type == KeyCapType.Buff);

        if(buffItems.Count > 0)
        {
            for(int i = 0; i < buffItems.Count; i++)
            {
                buffItems[i].CountDown();

                if (buffItems[i].count == 0)
                {
                    if(buffItems[i].tag == "TripleJump")
                    {
                        GetComponent<PlayerSelectKeyCap>().moveRange = 2;
                        photonView = PhotonView.Get(this);
                        photonView.RPC("SetMoveRangeRPC", RpcTarget.Others, 2);
                    }

                    items.Remove(buffItems[i]);
                }
            }
        }
    }

    public void CountDownToolItem()
    {
        if (current.type != KeyCapType.FalseTool && current.type != KeyCapType.Tool) return;

        var toolItem = items.FindAll(item=> item.type == KeyCapType.Tool);

        Debug.Log("Any ToolItem is : " + items.Any(item => item.type == KeyCapType.Tool));
        Debug.Log("Find ToolItem is : " + items.Find(item => item.type == KeyCapType.Tool));

        if(toolItem.Count > 0)
        {
            Debug.Log("Before Tool Count : " + toolItem[0].count);
            toolItem[0].CountDown();

            Debug.Log("After Tool Count : " + toolItem[0].count);
            if (toolItem[0].count == 0) items.Remove(toolItem[0]);
        }
        
    }

    public void RemoveItems()
    {
        items.RemoveRange(0, items.Count);
    }

    public void SetCurrent(KeyCap _current)
    {
        current = _current;
    }

    public void SetCurrentToUseTool()
    {
        keepCurrent.Enqueue(current);

        current = new ToolKeyCap(items.Find(item => item.type == KeyCapType.Tool));
    }

    public void SetCurrentAsKeep()
    {
        if (keepCurrent.Count > 0)
            current = keepCurrent.Dequeue();
    }

    //public void Use(PlayerBattle battleInfo)
    //{
    //    //Debug.LogWarning("Use");
    //    if (current.isActive)
    //    {
    //        Debug.Log("KeyCapTag in Use() : " + current.tag);
    //        current.Use(battleInfo);

    //        if (battleInfo.GetThisTurnPlayer.keyCaps.Current.type != KeyCapType.FalseTool)
    //            KeyBoard.instance.RemoveKeyCap(battleInfo.GetThisTurnPlayer.transform.position);

    //        //Setbuff();

    //        Debug.Log("Shield is Buffing : " + battleInfo.GetThisTurnPlayer.keyCaps.Isbuffing("Shield"));
    //        //battleInfo.GetThisTurnPlayer.UpdateUI();
    //        //battleInfo.GetThisTurnPlayer.UpdateUI("Shield", GetBuffKeyCap("Shield").count.ToString());
    //    }
    //}

    //public void KeyCapCountDown()
    //{
    //    if (buff.Count > 0)
    //    {
    //        foreach (KeyCap keycap in buff)
    //        {
    //            string tag = keycap.CountDown();
    //        }

    //        if(buff.Any(keycap => keycap.count == 0))
    //            buff.Remove(buff.Find(keycap => keycap.count == 0));
    //    }

    //    if (tool.Count > 0)
    //    {
    //        tool.Peek().CountDown();

    //        if(tool.Peek().count == 0)
    //        {
    //            tool.Dequeue();
    //        }
    //    }
    //}

    //public void CountDownBuff()
    //{
    //    if (buff.Count > 0)
    //    {
    //        for (int i = 0; i < buff.Count; i++)
    //        {
    //            buff[i].CountDown();

    //            if (buff[i].count == 0) buff.Remove(buff[i]);
    //        }
    //    }
    //}

    //public void CountDownTool()
    //{
    //    if (tool.Count > 0)
    //    {
    //        tool.Peek().CountDown();

    //        if (tool.Peek().count == 0) tool.Dequeue();
    //    }
    //}
}
