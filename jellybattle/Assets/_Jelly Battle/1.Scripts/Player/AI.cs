using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    GameManager gameManager;
    State currentState;

    private void Start()
    {
        gameManager = GameManager.instance;
        //currentState = gameManager.CurrentState;
    }

    public void ChoiceNextPosition()
    {
        Vector3[] availablePos = GetComponent<PlayerSelectKeyCap>().GetAvailableSelectRangeAsWorldPosition();

        bool isNothing = true;

        foreach(Vector3 pos in availablePos)
        {
            if(KeyBoard.instance.GetKeyCapCurrentWorldPositon(pos).tag != "Empty")
            {
                isNothing = false;
            }
        }

        string seed = (Time.time + Random.value).ToString();
        System.Random random = new System.Random(seed.GetHashCode());

        int randomIndex;

        do
        {
            randomIndex = random.Next(0, availablePos.Length);

            if (isNothing == true)
                break;
        }
        while (KeyBoard.instance.GetKeyCapCurrentWorldPositon(new Vector3(availablePos[randomIndex].x + 1, availablePos[randomIndex].y)).tag == "Empty" 
                || KeyBoard.instance.GetKeyCapCurrentWorldPositon(new Vector3(availablePos[randomIndex].x + 1, availablePos[randomIndex].y)).tag == "Danger");

        //Debug.Log("Index : " + randomIndex + ", Length : " + availablePos.Length);
        //Debug.Log("NextPos : " + availablePos[randomIndex]);

        GetComponent<Player>().SetNextPos(availablePos[randomIndex]);
    }
}
