using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardManager : MonoBehaviour
{
    public int width = 15, height = 5;

    [Header("Time")]
    public float startDelay = 3f;
    public float waitingTime = 5f;
    [SerializeField]
    float curTime = 0f;
    public Slider timeProgressbar;


    [Header("KeyCap")]
    public GameObject[] keyCap;
    public Item[] item;

    [Range(0, 100)]
    public float randomkeyCapPercent = 50.0f;

    int[] funcLine;
    int[,] keyBoard;

    GameObject[,] allKeyCaps;
    GameObject curClicked;

    [Header("Player")]
    public GameObject player;
    [Header("AI")]
    public GameObject AI;
    public GameObject AIHPBar;
    public Vector3 offset;

    private void Start()
    {
        allKeyCaps = new GameObject[width, height + 1];

        InitKeyboard();
        UpdateFunctionLine();

        InstantiateKeyboard();
        for (int i = 0; i < 2; i++)
            WaveKeyBoard();

        //InstantiatePlayer();
        InstantiateAI();

        curTime = waitingTime;
        //InvokeRepeating("WaveKeyBoard", startDelay, waitingTime);
    }

    private void Update()
    {
        //ClickKeyCap();

        TimeProgress();

        UpdateAI();
    }

    #region Time
    void TimeProgress()
    {
        if(startDelay > 0)
        {
            startDelay -= Time.deltaTime;
            return;
        }

        if(curTime < 0)
        {
            WaveKeyBoard();

            curTime = waitingTime;
        }

        ProgressingTimeBar();

        curTime -= Time.deltaTime;
    }

    void ProgressingTimeBar()
    {
        timeProgressbar.value = curTime / waitingTime + 0.1f;
    }

    #endregion

    #region KeyBoard
    void InitKeyboard()
    {
        funcLine = new int[width];
        keyBoard = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                keyBoard[x, y] = 0;
            }
        }

        for (int i = 0; i < width; i++)
        {
            funcLine[i] = 0;
        }
    }

    void UpdateFunctionLine()
    {
        string seed = (Time.time + Random.value).ToString();

        System.Random random = new System.Random(seed.GetHashCode());

        for (int i = 0; i < width; i++)
        {
            int randomKeyCap = (random.Next(0, 100) < randomkeyCapPercent) ? 1 : 0;
            funcLine[i] = (randomKeyCap == 1) ? random.Next(1, keyCap.Length) : 0;
        }
    }

    void InstantiateKeyboard()
    {
        for (int i = 0; i < width; i++)
        {
            GameObject newKeyCap = Instantiate(keyCap[funcLine[i]], new Vector3(-width / 2 + i, height / 2 + 2, 0), Quaternion.identity);

            allKeyCaps[i, height] = newKeyCap;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject newKeyCap = Instantiate(keyCap[keyBoard[x, y]], new Vector3(-width / 2 + x, -height / 2 + y, 0), Quaternion.identity);

                allKeyCaps[x, y] = newKeyCap;
            }
        }
    }

    void WaveKeyBoard()
    {
        ClearKeyBoard();

        UpdateKeyboard();

        InstantiateKeyboard();

        //SetPlayerWorldPosition();

        //CheckItem();
    }

    void ClearKeyBoard()
    {
        foreach (GameObject keycap in allKeyCaps)
        {
            Destroy(keycap);
        }
    }

    void UpdateKeyboard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height - 1; y++)
            {
                keyBoard[x, y] = keyBoard[x, y + 1];
            }
        }

        for (int i = 0; i < width; i++)
        {
            keyBoard[i, height - 1] = funcLine[i];
        }

        UpdateFunctionLine();
    }
    #endregion

    //#region Player
    //void InstantiatePlayer()
    //{
    //    Vector3 playerPos = player.GetComponent<PlayerManager>().position;

    //    //playerPos.y += 0.4f;

    //    player = Instantiate(player, PositionKeyboardToWorld(playerPos), Quaternion.identity);
    //}

    //void SetPlayerWorldPosition()
    //{
    //    Vector3 playerPos = player.GetComponent<PlayerManager>().position;

    //    //playerPos.y += 0.4f;

    //    player.transform.position = PositionKeyboardToWorld(playerPos);
    //}

    //void SetPlayerKeyboardPosition(Vector3 worldPos)
    //{
    //    Vector3 pos = worldPos;

    //    player.GetComponent<PlayerManager>().position = PositionWorldToKeyboard(pos);
    //}

    //void ClickKeyCap()
    //{
    //    Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    RaycastHit2D rayhit = Physics2D.Raycast(mousePos, Vector2.zero);

    //    if (rayhit.transform != null)
    //    {
    //        if (Input.GetMouseButtonDown(0))
    //        {
    //            GameObject selected = rayhit.transform.gameObject;

    //            Debug.Log(PositionWorldToKeyboard(selected.transform.position));

    //            AvailableClickRange(selected);

    //            //KeycapSelected(selected);

    //            //SetPlayerKeyboardPosition(selected.transform.position);
    //        }

    //    }

    //    ShowAvailableClickRange();
    //}

    //void AvailableClickRange(GameObject selected)
    //{
    //    Vector3 playerPos = PositionWorldToKeyboard(player.transform.position);
    //    Vector3 selectedPos = PositionWorldToKeyboard(selected.transform.position);

    //    if (selectedPos.x >= playerPos.x - 2 && selectedPos.x <= playerPos.x + 2 && selectedPos.y >= playerPos.y - 2 && selectedPos.y <= playerPos.y + 2)
    //    {
    //        KeycapSelected(selected);

    //        SetPlayerKeyboardPosition(selected.transform.position);
    //    }
    //}

    //void ShowAvailableClickRange()
    //{
    //    Vector3 playerPos = PositionWorldToKeyboard(player.transform.position);

    //    int posX = (int)playerPos.x - 1;
    //    int posY = (int)playerPos.y - 1;

    //    for(int x = posX - 2; x <= posX + 2; x++)
    //    {
    //        for (int y = posY - 2; y <= posY + 2; y++)
    //        {
    //            if (x >= 0 && x < width && y >= 0 && y < height)
    //                if (allKeyCaps[x, y] != curClicked)
    //                    allKeyCaps[x, y].GetComponentInChildren<ItemKeyCap>().Available.SetActive(true);
    //        }
    //    }
    //}

    //void KeycapSelected(GameObject select)
    //{
    //    if (curClicked != null)
    //    {
    //        curClicked.GetComponent<ItemKeyCap>().selected.SetActive(false);
    //    }
    //    //curClicked.GetComponent<SpriteRenderer>().color = new Color(0.3f, 0, 0, 0.8f);

    //    curClicked = select;

    //    select.GetComponent<ItemKeyCap>().selected.SetActive(true);
    //    //select.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.8f);
    //}

    //void CheckItem()
    //{
    //    Vector3 playerPos = player.GetComponent<PlayerManager>().position;

    //    int onKeyboardX = (int)playerPos.x - 1;
    //    int onKeyboardY = (int)playerPos.y - 1;

    //    if(keyCap[keyBoard[onKeyboardX, onKeyboardY]].GetComponent<ItemKeyCap>().item != null)
    //        AI.GetComponent<AIManager>().HP -= keyCap[keyBoard[onKeyboardX, onKeyboardY]].GetComponent<ItemKeyCap>().item.damage;
    //}

    //#endregion

    #region AI

    void InstantiateAI()
    {
        Vector3 AIPos = AI.GetComponent<AIManager>().position;

        AI = Instantiate(AI, PositionKeyboardToWorld(AIPos), Quaternion.identity);
    }

    void UpdateAI()
    {
        AI.GetComponentInChildren<Slider>().value = AI.GetComponent<AIManager>().HP / 100.0f;
        //AIHPBar.GetComponent<Slider>().value = AI.GetComponent<AIManager>().HP / 100.0f;

        AI.GetComponent<AIManager>().Dead();
    }

    #endregion

    #region Position Translate

    Vector3 PositionKeyboardToWorld(Vector3 pos)
    {
        return new Vector3(-width / 2 + pos.x - 1, -height / 2 + pos.y - 1);
    }

    Vector3 PositionWorldToKeyboard(Vector3 worldPos)
    {
        return new Vector3(worldPos.x + width / 2 + 1, worldPos.y + height / 2 + 1);
    }

    #endregion

    
}
