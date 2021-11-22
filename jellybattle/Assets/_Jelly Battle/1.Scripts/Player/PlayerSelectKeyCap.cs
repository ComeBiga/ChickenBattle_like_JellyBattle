using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectKeyCap : MonoBehaviour
{
    GameManager gameManager;
    KeyBoard keyBoard;

    Player player;
    PhotonView photonView;

    GameObject current;
    public int moveRange = 2;

    public bool isActive = true;
    private GameObject settingMenu;

    private void OnEnable()
    {
        gameManager = GameManager.instance;
        keyBoard = KeyBoard.instance;

        player = GetComponent<Player>();
        photonView = GetComponent<PhotonView>();

        settingMenu = GameObject.Find("Canvas").transform.Find("MenuPanel").gameObject;
        
    }

    public void HandleSelect()
    {
        Debug.Log("HandleSelect()");
        ClickKeyBoard();
        MarkAvailableMoveRange();
    }

    public void ClickKeyBoard()
    {
        //Debug.Log("Setting Menu's active is : " + settingMenu.activeSelf);
        if (settingMenu.activeSelf) return;

        switch(Launcher.instance.Gamemode)
        {
            case GameMode.Single:
                if (GetComponent<Player>().Type == PlayerType.AI) return;
                break;
            case GameMode.Multi:
                if (!photonView.IsMine) return;
                break;
        }

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D rayhit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (rayhit.transform != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameObject selected = rayhit.transform.gameObject;
                SelectKeyCap(selected);

                
            }
        }
    }

    void SelectKeyCap(GameObject selected)
    {
        if (keyBoard.IsKeyBoard(selected.gameObject) && IsAvailableSelect(selected.transform.position))
        {
            MarkNextPos(selected);

            current = selected;
            //player.nextPos = selected.transform.position;
            player.SetNextPos(selected.transform.position.x, selected.transform.position.y);
            player.photonView.RPC("SetNextPosRPC", RpcTarget.Others, selected.transform.position.x, selected.transform.position.y);

            AudioManager.instance.Play("Click1");
        }
        else
        {
            AudioManager.instance.Play("Click2");
        }
    }

    bool IsAvailableSelect(Vector3 selectPos)
    {
        Vector3 clickPos = selectPos;
        Vector3 playerPos = transform.position;

        if (clickPos.x >= playerPos.x - moveRange && clickPos.x <= playerPos.x + moveRange
            && clickPos.y >= playerPos.y - moveRange && clickPos.y <= playerPos.y + moveRange)
        {
            return true;
        }

        return false;
    }

    void MarkNextPos(GameObject selected)
    {
        if (current != null)
            current.GetComponent<Tile>().UnSelect();

        selected.GetComponent<Tile>().Select();
    }

    public void MarkAvailableMoveRange()
    {
        switch (Launcher.instance.Gamemode)
        {
            case GameMode.Single:
                if (GetComponent<Player>().Type == PlayerType.AI) return;
                break;
            case GameMode.Multi:
                if (!photonView.IsMine) return;
                break;
        }

        Vector3 playerPos = keyBoard.PositionWorldToKeyboard(transform.position);

        int playerPosX, playerPosY;
        playerPosX = (int)playerPos.x - 1;
        playerPosY = (int)playerPos.y - 1;

        //Debug.Log(playerPosX + ", " + playerPosY);
            
        for (int x = playerPosX - moveRange; x <= playerPosX + moveRange; x++)
        {
            for (int y = playerPosY - moveRange; y <= playerPosY + moveRange; y++)
            {
                if (x >= 0 && x < keyBoard.width && y >= 0 && y < keyBoard.height)
                {
                    keyBoard.keyCaps[x, y].GetComponent<Tile>().Available();
                }
            }
        }
    }

    /// <summary>
    /// 선택 가능한 위치들을 반환한다.
    /// </summary>
    /// <returns>선택가능한 KeyBoard Position</returns>
    public Vector3[] GetAvailableSelectRangeAsKeyBoardPosition()
    {
        List<Vector3> availablePos = new List<Vector3>();

        Vector3 playerPos = keyBoard.PositionWorldToKeyboard(transform.position);

        int playerPosX, playerPosY;
        playerPosX = (int)playerPos.x - 1;
        playerPosY = (int)playerPos.y - 1;

        //Debug.Log(playerPosX + ", " + playerPosY);

        for (int x = playerPosX - moveRange; x <= playerPosX + moveRange; x++)
        {
            for (int y = playerPosY - moveRange; y <= playerPosY + moveRange; y++)
            {
                if (x >= 0 && x < keyBoard.width && y >= 0 && y < keyBoard.height)
                {
                    availablePos.Add(new Vector3(x, y));
                }
            }
        }

        return availablePos.ToArray();
    }

    /// <summary>
    /// 선택 가능한 위치들을 반환한다.
    /// </summary>
    /// <returns>선택가능한 World Position</returns>
    public Vector3[] GetAvailableSelectRangeAsWorldPosition()
    {
        List<Vector3> availablePos = new List<Vector3>();

        Vector3 playerPos = keyBoard.PositionWorldToKeyboard(transform.position);

        int playerPosX, playerPosY;
        playerPosX = (int)playerPos.x;
        playerPosY = (int)playerPos.y;

        //Debug.Log(playerPosX + ", " + playerPosY);

        for (int x = playerPosX - moveRange; x <= playerPosX + moveRange; x++)
        {
            for (int y = playerPosY - moveRange; y <= playerPosY + moveRange; y++)
            {
                if (x >= 1 && x <= keyBoard.width && y >= 1 && y <= keyBoard.height)
                {
                    availablePos.Add(keyBoard.PositionKeyboardToWorld(new Vector3(x, y)));
                }
            }
        }

        return availablePos.ToArray();
    }
}
