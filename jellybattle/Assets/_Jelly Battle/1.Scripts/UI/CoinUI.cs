using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinUI : MonoBehaviour
{
    public TextMeshProUGUI coin;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("Coin"))
            coin.text = PlayerPrefs.GetInt("Coin").ToString();
        else
            PlayerPrefs.SetInt("Coin", 0);
    }
}
