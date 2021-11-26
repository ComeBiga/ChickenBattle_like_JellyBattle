using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System;

public enum State { Entrance, BeforeSelecting, Selecting, BeforeMoving, Moving, BeforeBattle, Battle, GameOver, Break }

public class GameManager : MonoBehaviourPunCallbacks
{
    #region Singleton
    public static GameManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    #endregion

    PlayerManager playerManager;
    KeyBoard keyboard;
    new PhotonView photonView;

    [SerializeField]
    State currentState = State.Entrance;
    public State CurrentState { get { return currentState; } }
    public void SetCurrentState(State nextState) => currentState = nextState;
    [PunRPC] private void SetServerCurrentStateRPC(int nextState) { currentState = (State)nextState; }
    public void SetServerCurrentState(State nextState)
    {
        SetServerCurrentStateRPC((int)nextState);
        photonView.RPC("SetServerCurrentStateRPC", RpcTarget.Others, (int)nextState);
    }

    [Header("Time")]
    public float startDelay = 3f;
    public float selectingTime = 5f;
    public float movingTime = 3f;
    public float gameOverTime = 5f;

    [SerializeField]
    float curTime = 0f;
    public void ResetTimer() => curTime = 0f;
    [SerializeField]
    float stateTime = 0f;
    public void SetStateTime(float time) => stateTime = time;
    public bool IsStateEnd => curTime >= stateTime;
    public void UpdateCurrentTime() => curTime += Time.deltaTime;

    //public TimeBar timeBar;
    public TMPro.TextMeshProUGUI timeClock;
    public TimeProgress timeProgress;
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject drawPanel;

    private void Start()
    {
        playerManager = PlayerManager.instance;
        Debug.Log(playerManager + "In Start()");
        keyboard = KeyBoard.instance;
        photonView = PhotonView.Get(this);

        SetStateTime(GameStateTime.instance.entranceTime);

        if(AudioManager.instance != null) AudioManager.instance.Play("BackGround");
    }

    private void Update()
    {

        switch (currentState)
        {
            case State.Entrance:
                EntranceState();
                break;
            case State.BeforeSelecting:
                BeforeSelectingState();
                break;
            case State.Selecting:
                SelectingState();
                break;
            case State.BeforeMoving:
                BeforeMovingState();
                break;
            case State.Moving:
                MovingState();
                break;
            case State.BeforeBattle:
                BeforeBattleState();
                break;
            case State.Battle:
                BattleState();
                break;
            case State.GameOver:
                GameOverState();
                break;
        }
    }

    delegate void TurnState();
    TurnState _currentState;

    private void UpdateState()
    {
        _currentState();
    }

    #region State
    void EntranceState()
    {
        UpdateCurrentTime();

        if(IsStateEnd)
        {
            ResetTimer();

            SetServerCurrentState(State.BeforeSelecting);
        }
    }

    void BeforeSelectingState()
    {
        //Debug.Log("BeforeSelectingState()");
        SetStateTime(GameStateTime.instance.selectingTime);

        //_currentState = SelectingState;
        SetCurrentState(State.Selecting);
    }

    void SelectingState()
    {
        UpdateCurrentTime();

        //if(onSelectingState != null) onSelectingState();
        Debug.Log("Before Select In SelectingState()");
        playerManager.GetMyLocalPlayer().GetComponent<PlayerSelectKeyCap>().HandleSelect();

        timeProgress.UpdateTimeProgress(curTime, stateTime);

        if (IsStateEnd)
        {
            if(PhotonNetwork.IsMasterClient)
                SetServerCurrentState(State.BeforeMoving);
            return;
        }
    }

    void BeforeMovingState()
    {
        ResetTimer();
        //Debug.Log("BeforeMovingState()");
        SetStateTime(GameStateTime.instance.movingTime);

        if (Launcher.instance.Gamemode == GameMode.Single)
            playerManager.GetAI().GetComponent<AI>().ChoiceNextPosition();

        playerManager.overlap.HandleOverlap();
        keyboard.UpdateKeyBoard();

        playerManager.AnimateAllLocalPlayer("IsRun");

        SetCurrentState(State.Moving);
    }

    void MovingState()
    {
        //Debug.Log("MovingState()");
        UpdateCurrentTime();
        //onMovingState();
        playerManager.MoveAllPlayer();

        if (IsStateEnd)
        {
            ResetTimer();
            //playerManager.ReadyUp();

            //NextState();
            if (PhotonNetwork.IsMasterClient)
                SetServerCurrentState(State.BeforeBattle);

            return;
        }

        //curTime -= Time.deltaTime;
    }

    void BeforeBattleState()
    {
        //Debug.Log("BeforeBattleState()");
        SetStateTime(GameStateTime.instance.selectingTime);

        playerManager.overlap.RandomPositionWinnerNew();        // PlayerBattle로...
        playerManager.RoundAllPlayerPosition();

        SetCurrentState(State.Battle);
    }

    void BattleState()
    {
        //Debug.Log("BattleState()");
        if (PhotonNetwork.IsMasterClient)
        {
            playerManager.battle.UpdateBattle();
        }
    }

    void GameOverState()
    {
        //if (Launcher.instance.Gamemode == GameMode.Single)
        //{
        //    AudioManager.instance.Play("Lose");
        //    losePanel.SetActive(true);
        //}
        //else
        //{
            switch (playerManager.GetMyLocalPlayer().Gameresult)
            {
                case GameResult.Win:
                    AudioManager.instance.Play("Win");
                    winPanel.SetActive(true);
                    if(PlayerPrefs.HasKey("Coin"))
                        PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + 20);
                    break;
                case GameResult.Lose:
                    AudioManager.instance.Play("Lose");
                    losePanel.SetActive(true);
                    break;
                case GameResult.Draw:
                    AudioManager.instance.Play("Draw");
                    drawPanel.SetActive(true);
                    if (PlayerPrefs.HasKey("Coin"))
                        PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + 5);
                break;
            }
        //}

        SetCurrentState(State.Break);

        //if (PhotonNetwork.IsMasterClient)
        //{
        //    UpdateCurrentTime();

        //    if (IsStateEnd)
        //    {
        //        ResetTimer();

        //        //GotoMenu();
        //    }
        //}
    }

    #endregion

    // Quit Button Script로 옮길 코드
    #region QuitButton

    public void QuitGame()
    {
        Application.Quit();
    }

    #endregion

    public void DropOutGame()
    {
        playerManager.SetPlayerLose(playerManager.GetMyLocalPlayer().userId);
        SetServerCurrentState(State.GameOver);
    }

    public void GotoMenu()
    {
        AudioManager.instance.Stop("BackGround");

        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.PlayerListOthers.Length > 0)
                PhotonNetwork.SetMasterClient(PhotonNetwork.PlayerListOthers[0]);
        }

        PhotonNetwork.LoadLevel(0);

        PhotonNetwork.LeaveRoom();

        //SceneManager.LoadScene(0);

        //PhotonNetwork.LeaveRoom();

        //StartCoroutine("GotoMenuCo");
    }

    IEnumerator GotoMenuCo()
    {
        if (PhotonNetwork.IsMasterClient) PhotonNetwork.SetMasterClient(PhotonNetwork.PlayerListOthers[0]);

        yield return new WaitForSeconds(1.0f);

        
    }

    public void GotoMenuSingleMode()
    {
        AudioManager.instance.Stop("BackGround");

        SceneManager.LoadScene(0);

        PhotonNetwork.LeaveRoom();
    }

    public void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Application.Quit();
        }
        else
        {

        }
    }
}

