using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateTime : MonoBehaviour
{
    #region Singleton
    public static GameStateTime instance = null;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    #endregion

    public float entranceTime = 1f;
    public float selectingTime = 3f;
    public float movingTime = 1.5f;
    public float gameOverTime = 5f;

    public float turnTime = 1.5f;
    public float overlapBattleTime = 1.5f;

    public float itemGetTime = 1f;
    public float emptyItemTime = .5f;
    public float itemUseTime = 1f;

    public static float currentTime = 0f;


}
