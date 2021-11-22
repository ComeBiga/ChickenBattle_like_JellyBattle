using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PlayerManager : MonoBehaviourPun
{
    #region Singleton
    public static PlayerManager instance = null;
    #endregion

    KeyBoard keyBoard;
    public PlayerBattle battle;
    public CheckSamePosition overlap;

    public int maxPlayer = 2;
    public Player[] players;
    //public GameObject[] playerGameObjects;
    public Vector2[] startPosition;
    [SerializeField]
    private int startPositionOffset = 4;

    public Slider[] HPbar;
    public Text[] ShieldText;
    public TMPro.TextMeshProUGUI[] nameText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        keyBoard = KeyBoard.instance;

        if (PhotonNetwork.IsMasterClient)
        {
            InitPlayers();
            InitStartPosition();

            switch(Launcher.instance.Gamemode)
            {
                case GameMode.Single:
                    InstantiatePlayersSingleMode();
                    break;
                case GameMode.Multi:
                    InstantiatePlayers();
                    break;
            }

            battle.InitPlayers(players);

            InitUI();
        }
    }

    public void InitPlayers()
    {
        players = new Player[maxPlayer];
    }

    private void InitStartPosition()
    {
        if (startPosition.Any(position => position.x > keyBoard.width))
        {
            Debug.LogError("Start Position is Out Of Range");
        }
    }

    private void InstantiatePlayersSingleMode()
    {
        //Player
        GameObject player = PhotonNetwork.Instantiate("DrawCharacter", keyBoard.PositionKeyboardToWorld(startPosition[0]), Quaternion.identity);
        players[0] = player.GetComponent<Player>();

        Vector2 startPos = keyBoard.PositionKeyboardToWorld(startPosition[0]);
        players[0].SetNextPos(startPos);
        
        players[0].SetUserNickName(PhotonNetwork.LocalPlayer.NickName);
        players[0].SetUserID(1);
        players[0].transform.GetChild(6).gameObject.SetActive(true);

        //AI
        GameObject enemy = PhotonNetwork.Instantiate("AI", keyBoard.PositionKeyboardToWorld(startPosition[1]), Quaternion.identity);
        players[1] = enemy.GetComponent<Player>();

        startPos = keyBoard.PositionKeyboardToWorld(startPosition[1]);
        players[1].SetNextPos(startPos);

        players[1].SetUserNickName("Duck");
        players[1].SetUserID(2);
        //players[0].transform.GetChild(6).gameObject.SetActive(false);
    }

    public void InstantiatePlayers()
    {
        Debug.Log("Players instantiated in InstantiatePlayers()");
        for (int i = 0; i < maxPlayer; i++)
        {
            GameObject player = PhotonNetwork.Instantiate("DrawCharacter", keyBoard.PositionKeyboardToWorld(startPosition[i]), Quaternion.identity, 0);
            players[i] = player.GetComponent<Player>();

            Vector2 startPos = keyBoard.PositionKeyboardToWorld(startPosition[i]);
            players[i].SetNextPos(startPos);

            players[i].GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.PlayerList[i]);
            players[i].SetUserNickName(PhotonNetwork.PlayerList[i].NickName);
            players[i].SetUserID(PhotonNetwork.PlayerList[i].ActorNumber);
            //Debug.Log(players[i].userNickName);
        }
    }

    [PunRPC]
    public void InitUIRPC()
    {
        GameObject[] clientPlayers = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < clientPlayers.Length; i++)
        {
            Player clientPlayer = clientPlayers[i].GetComponent<Player>();
            clientPlayer.UI = UIManager.instance.playerUI[i];
            clientPlayer.UI.Init();
            clientPlayer.UI.ownerName = clientPlayer.userNickName;
            clientPlayer.UI.SetPlayerName(clientPlayer.userNickName);
            clientPlayer.UI.id = clientPlayer.userId;

            if (clientPlayer.userId == PhotonNetwork.LocalPlayer.ActorNumber)
                clientPlayer.transform.GetChild(6).gameObject.SetActive(true);
        }
    }

    public void InitUI()
    {
        InitUIRPC();
        PhotonView pv = PhotonView.Get(this);
        pv.RPC("InitUIRPC", RpcTarget.Others);
    }

    //public bool PlayersBattle()
    //{
    //    curTime += Time.deltaTime;

    //    Player curTurn = currentTurn.GetComponent<Player>();

    //    curTurn.Battle();

    //    if (CheckPlayerDie()) return false;

    //    if (curTime > turnTime)
    //    {
    //        curTime = 0;

    //        curTurn.AfterTurn();

    //        if (playerDied)
    //        {
    //            GameManager.instance.ChangeStateGameOver();
    //            return false;
    //        }

    //        AllPlayersIdle();

    //        if(turnQueue.Count == 0)
    //        {
    //            return true;
    //        }

    //        currentTurn = turnQueue.Dequeue();
    //    }

    //    return false;
    //}

    //void MakeTurnQueue()
    //{
    //    turnQueue = new Queue<GameObject>();

    //    for(int i = 0; i< maxPlayer; i++)
    //    {
    //        if(players[i].GetComponent<Player>().CurKeyCap.type == KeyCapType.Ice)
    //        {
    //            turnQueue.Enqueue(players[i]);
    //        }
    //    }

    //    if (turnQueue.Count > 0)
    //    {
    //        currentTurn = turnQueue.Dequeue();
    //        return;
    //    }

    //    for(int i = 0; i < maxPlayer; i++)
    //    {
    //        turnQueue.Enqueue(players[i]);
    //    }

    //    currentTurn = turnQueue.Dequeue();
    //}

    //private bool CheckPlayerDie()
    //{
    //    foreach(GameObject player in players)
    //    {
    //        if(player.GetComponent<Player>().CurrentHP <= 0)
    //        {
    //            player.GetComponent<Player>().anim.DeadAnimation();
    //            GameManager.instance.ChangeStateGameOver();
    //            GameManager.instance.curTime = 0;
    //            return true;
    //        }
    //    }

    //    return false;
    //}

    public void RoundPlayerPosition()
    {
        foreach (Player player in players)
        {
            player.RoundPlayerPosition();
        }
    }

    //public void RemovePlayerKeyCaps()
    //{
    //    foreach(GameObject player in players)
    //    {
    //        KeyCap removeKeycap = player.GetComponent<Player>().CurKeyCap;

    //        KeyBoard.instance.RemoveKeyCap(removeKeycap.transform.position);
    //    }
    //}

    public void AllPlayersIdle()
    {
        foreach (Player player in players)
        {
            Debug.Log(player.name + "Idled");
            player.anim.IdleAnimation();
        }
    }

    #region PlayerInfo

    public Player GetPlayer(Player _player)
    {
        return GetPlayer(_player.userId);
    }

    public Player GetPlayer(int id)
    {
        foreach (Player player in players)
        {
            if (player.userId == id)
                return player;
        }

        return null;
    }

    public Player GetLocalPlayer(int actorNum)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach(GameObject player in players)
        {
            if (player.GetComponent<Player>().userId == actorNum)
                return player.GetComponent<Player>();
        }

        return null;
    }

    public Player GetMyLocalPlayer()
    {
        if (Launcher.instance.Gamemode == GameMode.Single)
            return GetLocalPlayer(1);
        else
            return GetLocalPlayer(PhotonNetwork.LocalPlayer.ActorNumber);
    }

    public Player[] GetAllPlayers()
    {
        return players;
    }

    public Player[] GetAllLocalPlayers()
    {
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        Player[] players = new Player[playerObjects.Length];

        for(int i = 0; i< players.Length; i++)
        {
            players[i] = playerObjects[i].GetComponent<Player>();
        }

        return players;
    }

    public Player[] GetAlivePlayers()
    {
        var alivePlayers = players.Where(player => player.IsDead != false).Select(player => player);

        return alivePlayers.ToArray();
    }

    public Player[] GetDeadPlayers()
    {
        var deadPlayers = players.Where(player => player.IsDead == true).Select(player => player);

        return deadPlayers.ToArray();
    }

    public void AnimatePlayer(int actorNum, string animationName)
    {
        foreach(Player player in players)
        {
            if(player.userId == actorNum)
            {
                player.anim.Animate(animationName, true);
            }
        }
    }

    public void AnimateAllPlayer(string name)
    {
        foreach(Player player in players)
        {
            player.anim.Animate(name, true);
        }
    }

    public void AnimateAllLocalPlayer(string name)
    {
        Player[] localPlayers = GetAllLocalPlayers();
        foreach (Player player in localPlayers)
        {
            player.anim.AnimateLocal(name, true);
        }
    }

    public void MoveAllPlayer()
    {
        Player[] players = GetAllLocalPlayers();

        foreach(Player player in players)
        {
            player.MovePlayerPositionSmooth();
        }
    }

    public void RoundAllPlayerPosition()
    {
        Player[] players = GetAllLocalPlayers();

        foreach (Player player in players)
        {
            player.RoundLocalPlayerPosition();
        }
    }

    public void SetAllPlayersCurrentKeyCap()
    {
        foreach(Player player in players)
        {
            player.keyCaps.SetCurrent(keyBoard.GetKeyCapCurrentWorldPositon(player.transform.position));
        }
    }

    public Player GetAI()
    {
        foreach(Player player in players)
        {
            if (player.Type == PlayerType.AI)
                return player;
        }

        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="winnerActNum">비겼을 땐 -1을 설정</param>
    public void SetPlayerWinner(int winnerActNum)
    {
        if(winnerActNum == -1)
        {
            foreach (Player player in players)
            {
                player.SetGameResult(GameResult.Draw);
            }
        }

        foreach(Player player in players)
        {
            if(player.userId == winnerActNum)
            {
                player.SetGameResult(GameResult.Win);
            }
            else
            {
                player.SetGameResult(GameResult.Lose);
            }
        }
    }

    public void SetPlayerLose(int loserActNum)
    {
        Player[] players = GetAllLocalPlayers();

        foreach (Player player in players)
        {
            if (player.userId == loserActNum)
            {
                player.SetGameResult(GameResult.Lose);
            }
            else
            {
                player.SetGameResult(GameResult.Win);
            }
        }
    }

    #endregion

}
