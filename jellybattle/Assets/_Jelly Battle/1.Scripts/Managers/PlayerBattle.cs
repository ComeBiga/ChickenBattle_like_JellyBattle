using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;

public enum BattleState { BeforeOverlapBattle, OverlapBattle, BeforeTurnBattle, TurnBattle, AfterTurn, AfterBattle }
public enum GameResult { Win, Lose, Draw }

public class PlayerBattle : MonoBehaviour
{
    GameManager gameManager;
    public CheckSamePosition overlap;
    private bool isOverlapDamaged = false;

    List<Player> players;
    public List<Player> GetAlivePlayers() { return players; }
    List<Player> deadPlayers;
    public void AddDeadPlayer(Player deadPlayer)
    {
        players.Remove(deadPlayer);
        deadPlayers.Add(deadPlayer);
    }

    BattleState currentBattleState = BattleState.BeforeOverlapBattle;
    void SetBattleState(BattleState next) => currentBattleState = next;

    int loopState = 0;
    Queue<Player> turn;
    public void InitTurn(Queue<Player> newTurn) => turn = newTurn;
    public void AddNextTurn(Player player) => turn.Enqueue(player);
    public int GetTurnCount => turn.Count;
    public void NextPlayerTurn() => turn.Dequeue();
    public Player GetThisTurnPlayer => turn.Peek();
    public Player[] GetNotTurnPlayers => players.Where(player => player != GetThisTurnPlayer).ToArray();

    public int winPlayerActNum;
    public GameResult gameResult;

    float curTime = 0;
    [SerializeField]
    float turnTime = 3f;
    float overlapBattleTime = 3f;
    public float OverlapBattleTime { get { return overlapBattleTime; } }

    public static Queue<Player> pendingHPbar = new Queue<Player>();

    private void Start()
    {
        gameManager = GameManager.instance;
        deadPlayers = new List<Player>();
    }

    public void InitPlayers(Player[] _players)
    {
        players = _players.ToList();
        //players = new List<Player>();

        //foreach (Player player in _players)
        //{
        //    players.Add(player);
        //}
    }

    public void UpdateBattle()
    {
        switch (currentBattleState)
        {
            case BattleState.BeforeOverlapBattle:
                BeforeOverlapBattleState();
                break;
            case BattleState.OverlapBattle:
                OverlapBattleState();
                break;
            case BattleState.BeforeTurnBattle:
                BeforeTurnBattleState();
                break;
            case BattleState.TurnBattle:
                TurnBattleState();
                break;
            case BattleState.AfterTurn:
                AfterTurnState();
                break;
            case BattleState.AfterBattle:
                AfterBattleState();
                break;
        }
    }

    void BeforeOverlapBattleState()
    {
        gameManager.SetStateTime(GameStateTime.instance.overlapBattleTime);

        if (overlap.isOverlaped == true)
            SetBattleState(BattleState.OverlapBattle);
        else
            SetBattleState(BattleState.BeforeTurnBattle);
    }

    public void OverlapBattleState()
    {
        gameManager.UpdateCurrentTime();

        if (isOverlapDamaged == false)
        {
            overlap.winPlayer.anim.AttackAnimation();

            foreach (Player loser in overlap.losePlayer)
            {
                loser.Damaged(overlap.overlapBattleDamage, 0);
            }

            isOverlapDamaged = true;
        }

        if (gameManager.IsStateEnd)
        {
            gameManager.ResetTimer();

            curTime = 0;
            overlap.isOverlaped = false;
            isOverlapDamaged = false;
            PlayerManager.instance.AllPlayersIdle();
            //Debug.Log("_________________HERE__________________");

            SetBattleState(BattleState.BeforeTurnBattle);
        }
    }

    public void BeforeTurnBattleState()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PlayerManager.instance.AllPlayersIdle();
            PlayerManager.instance.SetAllPlayersCurrentKeyCap();

            SetTurn();

            if (turn.Peek().keyCaps.Current.tag == "Empty")
                gameManager.SetStateTime(GameStateTime.instance.emptyItemTime);
            else
                gameManager.SetStateTime(GameStateTime.instance.turnTime);

            SetBattleState(BattleState.TurnBattle);
        }
    }

    public void TurnBattleState()
    {
        gameManager.UpdateCurrentTime();

        turn.Peek().UpdateUse(GetThisTurnPlayer, GetNotTurnPlayers);

        if (gameManager.IsStateEnd)
        {
            gameManager.ResetTimer();
            turn.Peek().SetCurrentState(UseState.Get);
            SetBattleState(BattleState.AfterTurn);
        }
    }

    private void AfterTurnState()
    {
        EffectManager.instance.DestroyEffects();

        turn.Peek().keyCaps.CountDownToolItem();
        turn.Peek().UpdateItemSlotUI();
        turn.Peek().keyCaps.SetCurrentAsKeep();

        GameObject[] allKeycaps = KeyBoard.instance.GetKeyCapAll();

        foreach (GameObject keycap in allKeycaps)
        {
            keycap.GetComponent<Tile>().UnAttacked();
            turn.Peek().SetKeyCapAttackedUI(keycap.transform.position.x, keycap.transform.position.y, false);
        }

        UpdateDamagedPlayers();

        if (players.Count <= 1)
        {
            if (players.Count == 0)
            {
                winPlayerActNum = -1;
                PlayerManager.instance.SetPlayerWinner(-1);
                gameManager.SetServerCurrentState(State.GameOver);
                //AudioManagerLocal.instance.Play("Draw");
                //PlayerWon();       //event
                return;
            }

            winPlayerActNum = players[0].userId;
            PlayerManager.instance.SetPlayerWinner(winPlayerActNum);
            //AudioManagerLocal.instance.PlayDifferentWithNickName("Win", "Lose", winPlayerActNum);
            gameManager.SetServerCurrentState(State.GameOver);
            //PlayerWon();        // event
            return;
        }

        turn.Dequeue();

        if (turn.Count > 0 && turn.Peek().keyCaps.Current.tag == "Empty")
            gameManager.SetStateTime(1f);
        else
            gameManager.SetStateTime(GameStateTime.instance.turnTime);

        PlayerManager.instance.AllPlayersIdle();

        if (turn.Count == 0)
        {
            SetBattleState(BattleState.AfterBattle);
        }
        else
        {
            SetBattleState(BattleState.TurnBattle);
        }
    }

    private void AfterBattleState()
    {
        foreach (Player player in players)
        {
            player.keyCaps.CountDownBuffItem();

            player.UpdateItemSlotUI();

            //player.keyCaps.CountDownTool();
        }

        SetBattleState(BattleState.BeforeOverlapBattle);
        gameManager.SetServerCurrentState(State.BeforeSelecting);
    }

    /// <summary>
    /// 공격 차례 정하는 함수
    /// </summary>
    public void SetTurn()
    {
        Debug.Log("In SetTurn");
        if (PhotonNetwork.IsMasterClient)
        {
            turn = new Queue<Player>();
            List<Player> playersforTurn = new List<Player>(); // players를 복사했기 때문에 현재 함수내에서 players의 값을 복사할려고 하면 안됨.
            foreach (Player player in players)
            {
                playersforTurn.Add(player);
            }

            var turnController = playersforTurn.FirstOrDefault(player => player.keyCaps.Current.controlTurn == ControlTurn.Steal);

            if (turnController != null)
            {
                turn.Enqueue(turnController);
                return;
            }

            // Turn Change(for Shield)
            var turnChanger = playersforTurn.FindAll(player => player.keyCaps.Current.controlTurn == ControlTurn.Change);

            if (turnChanger != null)
                foreach (var _turnChanger in turnChanger)
                {
                    turn.Enqueue(_turnChanger);
                    playersforTurn.Remove(_turnChanger);
                }

            // Normal Turn
            foreach (Player player in playersforTurn)
            {
                //Debug.Log("Player has tool to set turn : " + player.keyCaps.tool + "(" + player.userNickName + ")");

                // Tool Turn
                if (player.keyCaps.HasTool)
                {
                    Debug.Log("Set Tool Turn");
                    player.keyCaps.SetCurrentToUseTool();
                    turn.Enqueue(player);
                }
                turn.Enqueue(player);
            }
        }
    }

    private void UpdateDamagedPlayers()
    {
        while (pendingHPbar.Count > 0)
        {
            Debug.Log("UpdateDamagedPlayers()");
            Player damagedPlayer = pendingHPbar.Dequeue();

            Debug.Log("UPdateHPBar()");
            damagedPlayer.UpdateHPBar();

            if (damagedPlayer.IsDead)
            {
                players.Remove(damagedPlayer);
                deadPlayers.Add(damagedPlayer);
            }
        }
    }

    
}