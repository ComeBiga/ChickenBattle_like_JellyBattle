using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum UseState { Get, BeforeUse, Using, Activate }
public enum PlayerType { Player, AI }

public class Player : MonoBehaviour
{
    #region Component fields
    GameManager gameManager;
    KeyBoard keyBoard;
    public PlayerAnimation anim;
    public PhotonView photonView;
    #endregion

    #region PlayerInfo
    // Player Info
    [SerializeField]
    private PlayerType type;
    public PlayerType Type { get { return type; } }

    public int userId;
    [PunRPC] private void SetUserIDRPC(int id)
    {
        userId = id;
    }
    public void SetUserID(int id)
    {
        SetUserIDRPC(id);
        photonView.RPC("SetUserIDRPC", RpcTarget.Others, id);
    }

    public string userNickName;
    //public TMPro.TextMeshProUGUI nameText;
    [PunRPC] public void SetUserNickNameRPC(string nickname)
    {
        userNickName = nickname;
    }
    public void SetUserNickName(string nickname)
    {
        SetUserNickNameRPC(nickname);
        photonView.RPC("SetUserNickNameRPC", RpcTarget.Others, nickname);
    }

    // NextPosition
    [SerializeField] PlayerPosition position;
    private Vector3 nextPos;
    public Vector3 NextPos
    {
        get
        {
            return nextPos;
        }
    }
    [PunRPC]
    private void SetNextPosRPC(float x, float y)
    {
        nextPos = new Vector2(x, y);
    }
    public void SetNextPos(Vector3 nextPos) { SetNextPosRPC(nextPos.x, nextPos.y);  photonView.RPC("SetNextPosRPC", RpcTarget.Others, nextPos.x, nextPos.y); }
    public void SetNextPos(float x, float y) { SetNextPosRPC(x, y);  photonView.RPC("SetNextPosRPC", RpcTarget.Others, x, y); }
    public void SetNextPosLocal(Vector3 nextPos) { this.nextPos = nextPos; }

    // State
    UseState currentUseState = UseState.Get;
    public void SetCurrentState(UseState next) => currentUseState = next;

    // KeyCap
    public PlayerKeyCaps keyCaps;

    // HP
    [SerializeField] Health health;
    private int maxHP = 100;
    [SerializeField]
    private int currentHP;
    public int CurrentHP
    {
        get
        {
            return currentHP;
        }
        set
        {
            currentHP = value;
            if (currentHP > maxHP) currentHP = maxHP;
        }
    }
    //public Slider HPbar;
    public bool IsDead { get { return currentHP <= 0; } }

    public float moveSpeed = 5.0f;
    [Range(1, 5)] public int moveRange = 2;

    public PlayerUI UI;

    [SerializeField]
    GameResult gameResult;
    public GameResult Gameresult { get { return gameResult; } }

    private void Awake()
    {
        gameManager = GameManager.instance;
        keyBoard = KeyBoard.instance;
        photonView = PhotonView.Get(this);
        keyCaps = GetComponent<PlayerKeyCaps>();

        position = new PlayerPosition(transform.position);
        health = GetComponent<Health>();

        InitPlayer();
    }

    #endregion

    #region Initialization

    public void InitPlayer()
    {
        currentHP = maxHP;
    }

    #endregion

    #region MovePosition
    public void MovePlayerPositionSmooth()
    {
        transform.position = Vector3.Lerp(transform.position, NextPos, Time.deltaTime * moveSpeed);

        if(Mathf.Approximately(transform.position.x, NextPos.x) && Mathf.Approximately(transform.position.y, NextPos.y))
        {
            transform.position = NextPos;
        }

        if (transform.position == NextPos)
        {
            // Animation
            anim.IdleAnimationLocal();
        }
    }

    [PunRPC]
    private void RoundPlayerPositionRPC()
    {
        // Position interpolation
        Vector3 playerPos = transform.position;
        playerPos = transform.position = new Vector3(Mathf.Round(playerPos.x), Mathf.Round(playerPos.y));
        //playerPos = keyBoard.PositionWorldToKeyboard(playerPos); 왜 있는지 모르겠는 코드 ***

        // CurrentKeyCapSet
        keyCaps.SetCurrent(keyBoard.GetKeyCapCurrentWorldPositon(transform.position));
    }

    public void RoundPlayerPosition()
    {
        RoundPlayerPositionRPC();
        photonView.RPC("RoundPlayerPositionRPC", RpcTarget.Others);
    }

    public void RoundLocalPlayerPosition()
    {
        Vector3 playerPos = transform.position;
        playerPos = transform.position = new Vector3(Mathf.Round(playerPos.x), Mathf.Round(playerPos.y));
    }

    [PunRPC]
    private void SetPositionRPC(float x, float y)
    {
        transform.position = new Vector2(x, y);
    }

    public void SetPosition(Vector3 pos)
    {
        SetPositionRPC(pos.x, pos.y);
        photonView.RPC("SetPositionRPC", RpcTarget.Others, pos.x, pos.y);
    }

    public Vector3 GetCurrentKeyBoardPosition()
    {
        return keyBoard.PositionWorldToKeyboard(transform.position);
    }

    public void SetCurrentKeyCap(Vector3 worldPos)
    {
        keyCaps.SetCurrent(keyBoard.GetKeyCapCurrentWorldPositon(worldPos));
    }

    #endregion

    #region Battle

    public void UpdateUse(Player player, Player[] enemies)
    {
        switch(currentUseState)
        {
            case UseState.Get:
                GetState();
                break;
            case UseState.BeforeUse:
                BeforeUseState();
                break;
            case UseState.Using:
                UsingState(player, enemies);
                break;
            case UseState.Activate:
                ActivateState();
                break;
        }
    }

    void GetState()
    {
        if (keyCaps.Current.type != KeyCapType.FalseTool)
            KeyBoard.instance.RemoveKeyCap(transform.position);

        //AudioManagerLocal.instance.Play("GetEm");

        gameManager.SetStateTime(GameStateTime.instance.itemGetTime);
        SetCurrentState(UseState.BeforeUse);
    }

    void BeforeUseState()
    {
        gameManager.UpdateCurrentTime();

        if(gameManager.IsStateEnd)
        {
            gameManager.ResetTimer();

            float useTime = 0f;
            if (keyCaps.Current.tag == "Empty")
                useTime = GameStateTime.instance.emptyItemTime;
            else
                useTime = GameStateTime.instance.itemUseTime;

            gameManager.SetStateTime(useTime);

            SetCurrentState(UseState.Using);
        }
    }

    public void UsingState(Player player, Player[] enemies)
    {
        if (keyCaps.Current.isActive)
        {
            Debug.Log("Current KeyCap : " + keyCaps.Current);
            keyCaps.Current.Use(player, enemies);
            
            if(keyCaps.Current.type == KeyCapType.Buff || keyCaps.Current.type == KeyCapType.Tool)
            {
                keyCaps.AddItem();
                UpdateItemSlotUI();
            }
        }

        SetCurrentState(UseState.Activate);
    }

    void ActivateState()
    {

    }

    #endregion

    #region Item
    [PunRPC]
    private void UpdateBuffSlotUIRPC(int imageCode, string count, int slotIndex)
    {
         UI.slots[slotIndex].AddBuff(imageCode, count.ToString());
    }

    public void UpdateBuffSlotUI(int imageCode, string count, int slotIndex)
    {
        UpdateBuffSlotUIRPC(imageCode, count, slotIndex);
        photonView.RPC("UpdateBuffSlotUIRPC", RpcTarget.Others, imageCode, count, slotIndex);
    }

    [PunRPC]
    private void ClearSlotRPC(int slotIndex)
    {
        UI.slots[slotIndex].ClearSlot();
    }

    public void ClearSlot(int slotIndex)
    {
        ClearSlotRPC(slotIndex);
        photonView.RPC("ClearSlotRPC", RpcTarget.Others, slotIndex);
    }

    public void UpdateItemSlotUI()
    {
        int index = 0;

        for(int i = 0; i < keyCaps.items.Count; i++)
        {
            UpdateBuffSlotUI(keyCaps.items[i].imageCode, keyCaps.items[i].count.ToString(), i);
            index++;
        }

        for(int i = index; i< UI.slots.Length; i++)
        {
            ClearSlot(i);
        }
    }
    #endregion

    #region Health
    [PunRPC]
    private void UpdateHPBarRPC()
    {
        //Debug.Log("SetHPBarRPC()");
        //HPbar.GetComponent<SlideBar>().UpdateFillBar(currentHP, maxHP);
        Debug.Log(UI.HPbar);
        UI.SetHPBar(currentHP, maxHP);
    }

    public void UpdateHPBar()
    {
        Debug.Log("In UpdateHPBar");
        //HPbar.GetComponent<SlideBar>().UpdateFillBar(currentHP, maxHP);
        UpdateHPBarRPC();
        photonView.RPC("UpdateHPBarRPC", RpcTarget.Others);
    }

    [PunRPC]
    private void DamagedRPC(int damage, int type)
    {
        currentHP -= damage;
    }

    public void Damaged(int damage, int type)
    {
        if (keyCaps.Isbuffing("Shield") == false)
        {
            //photonView.RPC("TakeDamage", RpcTarget.AllViaServer, damage);
            //photonView.RPC("DamagedRPC", RpcTarget.AllViaServer, damage, type);

            DamagedRPC(damage, type);
            photonView.RPC("DamagedRPC", RpcTarget.Others, damage, type);

            if (IsDead)
            {
                anim.DeadAnimation();
            }
            else
            {
                switch (type)
                {
                    case 0:
                        Debug.Log("Hit Animation()");
                        anim.HitAnimation();
                        break;
                    case 1:
                        anim.DeadAnimation();
                        break;
                }
            }

            PlayerBattle.pendingHPbar.Enqueue(this);
            EffectManager.instance.DamageEffect(transform.position, damage);
        }
    }

    [PunRPC]
    private void RecoverRPC(int HP)
    {
        CurrentHP += HP;
        UI.SetHPBar(currentHP, maxHP);
        //HPbar.GetComponent<SlideBar>().UpdateFillBar(currentHP, maxHP);
    }

    public void Recover(int HP)
    {
        RecoverRPC(HP);
        photonView.RPC("RecoverRPC", RpcTarget.Others, HP);
    }

    #endregion

    #region UI
    [PunRPC]
    private void SetKeyCapAttackedUIRPC(float x, float y, bool value)
    {
        if (value == true)
        {
            KeyBoard.instance.GetKeyCapCurrentWorldPositon(new Vector3(x, y)).GetComponent<Tile>().Attacked();
        }
        else
        {
            //Debug.Log("keyCap(" + x + ", " + y + ") : " + KeyBoard.instance.GetKeyCapCurrentWorldPositon(new Vector3(x, y)));
            KeyBoard.instance.GetKeyCapCurrentWorldPositon(new Vector3(x, y)).GetComponent<Tile>().UnAttacked();
        }
    }

    public void SetKeyCapAttackedUI(float x, float y, bool value)
    {
        //SetKeyCapAttackedUIRPC(x, y, value);
        photonView.RPC("SetKeyCapAttackedUIRPC", RpcTarget.Others, x, y, value);
    }

    [PunRPC]
    private void SetGameResultRPC(int result)
    {
        gameResult = (GameResult)result;
    }

    public void SetGameResult(GameResult result)
    {
        SetGameResultRPC((int)result);
        photonView.RPC("SetGameResultRPC", RpcTarget.Others, (int)result);
    }

    #endregion
}
