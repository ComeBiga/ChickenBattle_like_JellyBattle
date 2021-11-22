using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBoard : MonoBehaviourPunCallbacks
{ 
    #region Singleton
    public static KeyBoard instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    #endregion

    GameManager gameManager;

    public bool mobileScreen = false;
    public float mobileSizeRatio = .5f;

    public int width = 15;
    public int height = 5;

    [Header("KeyCap")]
    int[,] keyboard;
    int[] funcLine;
    [Range(0, 2)]
    public float funclineOffset = 2f;

    public Vector2 keyCapSize;
    public GameObject[,] keyCaps;
    public GameObject[] types;

    [Range(1, 5)]
    public int startWave = 3;
    [Range(0, 100)]
    public float fillKeycapPercent = 45.0f;

    [Header("Player")]
    public GameObject player;   // KeyBoard.cs 에서 없어도 될 변수 ***

    private void Start()
    {
        if(GameManager.instance != null)
            gameManager = GameManager.instance;

        //InitKeyboard();
        InitKeyBoardOfMasterClient();

        //InitPlayer();
    }

    #region Initialization
    // 안 쓰는 함수 ***
    void InitKeyboard()
    {
        keyboard = new int[width, height];
        funcLine = new int[width];

        keyCaps = new GameObject[width, height + 1];

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                keyboard[x, y] = 0;
            }
        }

        for(int i = 0; i< width; i++)
        {
            funcLine[i] = 0;
        }

        RandomKeyCapFunctionLine();

        for(int i = 0; i < 2; i++)
        {
            UpdateKeyBoardValue();
        }

        UpdateKeyBoardGameObjects();
    }

    void InitKeyBoardOfMasterClient()
    {
        keyboard = new int[width, height];
        funcLine = new int[width];

        keyCaps = new GameObject[width, height + 1];

        if (PhotonNetwork.IsMasterClient)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    keyboard[x, y] = 0;
                }
            }

            for (int i = 0; i < width; i++)
            {
                funcLine[i] = 0;
            }

            RandomKeyCapFunctionLine();

            for (int i = 0; i < startWave; i++)
            {
                UpdateKeyBoardValue();
            }

            SendInitValue();

            UpdateKeyBoardGameObjects();
        }
    }
    #endregion

    #region RPC
    public void SendInitValue()
    {
        int[] keyboardArrayFlat = new int[width * height];

        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                keyboardArrayFlat[x * height + y] = keyboard[x, y];
            }
        }

        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("InitValueRPC", RpcTarget.Others, keyboardArrayFlat, funcLine);
    }

    public void SendUpdateValue()
    {
        int[] keyboardArrayFlat = new int[width * height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                keyboardArrayFlat[x * height + y] = keyboard[x, y];
            }
        }

        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("UpdateValueRPC", RpcTarget.Others, keyboardArrayFlat, funcLine);
    }

    [PunRPC]
    void InitValueRPC(int[] _keyboard, int[] _funcLine)
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                keyboard[x, y] = _keyboard[x * height + y];
            }
        }

        funcLine = _funcLine;

        UpdateKeyBoardGameObjects();
    }

    [PunRPC]
    void UpdateValueRPC(int[] _keyboard, int[] _funcLine)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                keyboard[x, y] = _keyboard[x * height + y];
            }
        }

        funcLine = _funcLine;
    }

    [PunRPC]
    void UpdateKeyBoardGameObjectsRPC()
    {
        UpdateKeyBoardGameObjects();
    }

    #endregion

    #region Update
    public void UpdateKeyBoard()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            UpdateKeyBoardValue();

            SendUpdateValue();
        }

        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("UpdateKeyBoardGameObjectsRPC", RpcTarget.All);
    }
    #endregion

    #region Keyboard Value

    void UpdateKeyBoardValue()
    {
        WaveKeyBoardValue();

        RandomKeyCapFunctionLine();
    }

    void RandomKeyCapFunctionLine()
    {
        string seed = (Time.time + Random.value).ToString();

        System.Random random = new System.Random(seed.GetHashCode());

        for (int i = 0; i < width; i++)
        {
            bool isItemKeyCap = (random.Next(0, 100) < fillKeycapPercent) ? true : false;

            int randomType = (isItemKeyCap) ? random.Next(1, types.Length) : 0;

            if(randomType == 12)
            {
                randomType = (random.Next(0, 100) > 90) ? randomType : 3;
            }

            funcLine[i] = randomType;
        }
    }

    void WaveKeyBoardValue()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height - 1; y++)
            {
                keyboard[x, y] = keyboard[x, y + 1];
            }
        }

        for (int i = 0; i < width; i++)
        {
            keyboard[i, height - 1] = funcLine[i];
        }
    }

    /// <summary>
    /// Keyboard에서 해당 keycap의 값을 0으로 설정한다.
    /// </summary>
    /// <param name="pos">해당 keycap의 world position값</param>
    public void RemoveKeyCap(Vector3 pos)
    {
        Vector3 removePos = PositionWorldToKeyboard(pos);

        int removePosX, removePosY;
        removePosX = (int)removePos.x - 1;
        removePosY = (int)removePos.y - 1;

        keyboard[removePosX, removePosY] = 0;

        RemoveKeyCap(removePosX, removePosY);
    }

    [PunRPC]
    private void RemoveKeyCapRPC(int removePosX, int removePosY)
    {
        keyCaps[removePosX, removePosY].transform.GetChild(0).gameObject.SetActive(false);
    }

    private void RemoveKeyCap(int removePosX, int removePosY)
    {
        RemoveKeyCapRPC(removePosX, removePosY);
        PhotonView pv = PhotonView.Get(this);
        pv.RPC("RemoveKeyCapRPC", RpcTarget.Others, removePosX, removePosY);
    }

    #endregion

    #region KeyCap GameObject
    void UpdateKeyBoardGameObjects()
    {
        DestroyKeyCaps();

        if (mobileScreen) InstantiateKeyCapsToMobile();
        else InstantiateKeyCaps();
    }
    
    void DestroyKeyCaps()
    {
        foreach(GameObject keycap in keyCaps)
        {
            if(keycap != null)
                Destroy(keycap);
        }
    }

    void InstantiateKeyCaps()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                GameObject keycap = types[keyboard[x, y]];
                Vector3 keycapPos = new Vector3(-width / 2 + x, -height / 2 + y);

                keyCaps[x, y] = Instantiate(keycap, keycapPos, Quaternion.identity);
                keyCaps[x, y].transform.localScale = keyCapSize;
            }
        }

        for(int i = 0; i < width; i++)
        {
            GameObject keycap = types[funcLine[i]];
            Vector3 keycapPos = new Vector3(-width / 2 + i, height / 2 + 2);

            keyCaps[i, height] = Instantiate(keycap, keycapPos, Quaternion.identity);
            keyCaps[i, height].transform.localScale = keyCapSize;
        }
    }

    void InstantiateKeyCapsToMobile()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject keycap = types[keyboard[x, y]];
                Vector3 keycapPos = new Vector3(-height / 2 + y, width / 2 - x);

                keyCaps[x, y] = Instantiate(keycap, keycapPos, Quaternion.identity);
                keyCaps[x, y].transform.localScale = keyCapSize;
            }
        }

        for (int i = 0; i < width; i++)
        {
            GameObject keycap = types[funcLine[i]];
            Vector3 keycapPos = new Vector3(height / 2 + funclineOffset, width / 2 - i);

            keyCaps[i, height] = Instantiate(keycap, keycapPos, Quaternion.identity);
            keyCaps[i, height].transform.localScale = keyCapSize;
        }
    }

    #endregion

    public GameObject[] GetKeyCapAll()
    {
        List<GameObject> allKeyCap = new List<GameObject>();

        foreach(GameObject keycap in keyCaps)
        {
            allKeyCap.Add(keycap);
        }

        return allKeyCap.ToArray();
    }
     
    public KeyCap GetKeyCapCurrentWorldPositon(Vector3 _currentPos)
    {
        Vector3 currentPos = PositionWorldToKeyboard(_currentPos);

        return keyCaps[(int)currentPos.x - 1, (int)currentPos.y - 1].GetComponent<Tile>().keyCap;
    }

    public bool IsKeyBoard(GameObject selected)
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                if(keyCaps[x, y] == selected)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool IsKeyBoard(Vector3 _currentPos)
    {
        Vector3 currentPos = PositionWorldToKeyboard(_currentPos);

        if(currentPos.x >= 1 && currentPos.x <= width && currentPos.y >= 1 && currentPos.y <= height)
        {
            return true;
        }

        return false;
    }

    public void DestroyKeyBoard()
    {
        foreach(GameObject keyCap in keyCaps)
        {
            DestroyImmediate(keyCap);
        }
    }

    #region Position Translate

    public Vector3 PositionKeyboardToWorld(Vector3 pos)
    {
        if (mobileScreen)
        {
            return new Vector3(-height / 2 + pos.y - 1, width / 2 - pos.x + 1);
        }
        return new Vector3(-width / 2 + pos.x - 1, -height / 2 + pos.y - 1);
    }

    public Vector3 PositionWorldToKeyboard(Vector3 worldPos)
    {
        if (mobileScreen) return new Vector3(width / 2 - worldPos.y + 1, worldPos.x + height / 2 + 1);
        return new Vector3(worldPos.x + width / 2 + 1, worldPos.y + height / 2 + 1);
    }

    #endregion
}
