using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Singleton

    public static UIManager instance = null;

    public void Awake()
    {
        if (instance == null)
            instance = this;
    }

    #endregion

    public List<PlayerUI> playerUI;

    public Sprite[] buffImages;

    public PlayerUI GetPlayerUI(string nickname)
    {
        return playerUI.Find(ui => ui.ownerName == nickname);
    }

    public PlayerUI GetPlayerUIwithID(int id)
    {
        return playerUI.Find(ui => ui.id == id);
    }
}
