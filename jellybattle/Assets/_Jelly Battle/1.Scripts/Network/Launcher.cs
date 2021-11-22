using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public enum GameMode { Single, Multi }

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher instance;

    #region Private Serializable Fields

    private GameMode gamemode;
    public GameMode Gamemode
    {
        get { return gamemode; }
    }

    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
    [SerializeField]
    private byte maxPlayersPerRoom = 2;

    public int startPlayerCount = 2;
    public int currentPlayerCount = 0;

    [Tooltip("The Ui Panel to let the user enter name, connect and play")]
    [SerializeField]
    private GameObject controlPanel;
    [Tooltip("The UI Label to inform the user that the connection is in progress")]
    [SerializeField]
    private GameObject progressLabel;
    [SerializeField]
    private GameObject connectingLabel;

    #endregion

    #region Private Fields

    string gameVersion = "1";

    #endregion

    #region MonoBehaviour CallBacks

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }


        DontDestroyOnLoad(gameObject);

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        Debug.Log("Connecting to Server");

        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        connectingLabel.SetActive(true);
        progressLabel.SetActive(false);
        controlPanel.SetActive(false);
    }

    private void Init()
    {
        MainMenuUI UIs = GameObject.Find("Canvas").GetComponent<MainMenuUI>();
        connectingLabel = UIs.connectingLabel;
        progressLabel = UIs.progressPanel;
        controlPanel = UIs.controlPanel;

        controlPanel.transform.Find("SinglePlayButton").GetComponent<Button>().onClick.AddListener(ConnectSingleMode);
        controlPanel.transform.Find("MultiPlayButton").GetComponent<Button>().onClick.AddListener(Connect);

        progressLabel.transform.Find("CancelButton").GetComponent<Button>().onClick.AddListener(CancelFindOpponent);

        //Button[] buttons = controlPanel.GetComponentsInChildren<Button>();
        //buttons[0].onClick.AddListener(ConnectSingleMode);
        //buttons[1].onClick.AddListener(Connect);
        //buttons[2].onClick.AddListener(QuitGame);
        //progressLabel.GetComponentInChildren<Button>().onClick.AddListener(CancelFindOpponent);
        controlPanel.GetComponentInChildren<TMP_InputField>().onValueChanged.AddListener(controlPanel.GetComponentInChildren<TMP_InputField>().transform.GetComponent<PlayerNameInputField>().SetPlayerName);


        GameObject quitButton = UIs.quitButton;
        quitButton.GetComponent<Button>().onClick.AddListener(QuitGame);
    }

    #endregion

    #region public Methods

    public void ConnectSingleMode()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 1 });
            gamemode = GameMode.Single;
        }
    }

    public void Connect()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            gamemode = GameMode.Multi;
            //progressLabel.SetActive(true);
            StartCoroutine("ViewProgressPanel");
            controlPanel.SetActive(false);

            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            Debug.Log("Isn't ConnectedAndReady!!");
        }
    }

    IEnumerator ViewProgressPanel()
    {
        progressLabel.SetActive(true);
        progressLabel.transform.GetChild(0).gameObject.SetActive(true);
        progressLabel.transform.GetChild(1).gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);

        if(progressLabel != null) progressLabel.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void CancelFindOpponent()
    {
        //PhotonNetwork.LoadLevel(0);
        PhotonNetwork.LeaveRoom();
    }

    #endregion

    #region MonoBehaviourPunCallbacks Callbacks


    public override void OnConnectedToMaster()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");

        connectingLabel.SetActive(false);
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);

        //PhotonNetwork.JoinRandomRoom();
        currentPlayerCount = PhotonNetwork.CountOfPlayers;
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        connectingLabel.SetActive(true);
        progressLabel.SetActive(false);
        controlPanel.SetActive(false);

        PhotonNetwork.Reconnect();

        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");

        if(gamemode == GameMode.Single)
        {
            SceneManager.LoadScene(1);
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount == startPlayerCount)
        {
            //SceneManager.LoadScene(1);
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.Log("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            //PhotonNetwork.LoadLevel(1);
        }
    }

    public override void OnJoinedLobby()
    {

    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player other)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.LoadLevel(1);
                //LoadArena();
            }
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player other)
    {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
        }

        //GameManager.instance.SetCurrentState(State.Break);

        //AudioManager.instance.Stop("BackGround");

        //PhotonNetwork.LoadLevel(0);

        //PhotonNetwork.LeaveRoom();
    }

    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
        Init();

        connectingLabel.SetActive(false);
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }

    #endregion

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OnApplicationPause(bool pause)
    {
        if(pause)
        {
            
        }
        else
        {

        }
    }
}
