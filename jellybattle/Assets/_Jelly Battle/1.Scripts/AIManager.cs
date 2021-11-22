using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public Vector3 position;
    Vector3 nextPos;

    public int HP = 100;

    void SelectNextPosition()
    {
        string seed = Time.time.ToString();

        System.Random random = new System.Random(seed.GetHashCode());

        // 다음 위치를 정하는 코드
    }

    public void Dead()
    {
        if(HP <= 0)
            GetComponent<Animator>().SetBool("IsDead", true);
    }
}
