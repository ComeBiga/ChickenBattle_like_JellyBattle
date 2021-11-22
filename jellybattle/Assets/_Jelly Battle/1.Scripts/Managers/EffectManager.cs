using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    #region Singleton

    public static EffectManager instance = null;

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    #endregion

    PhotonView photonView;

    public GameObject[] effects;

    private void Start()
    {
        photonView = PhotonView.Get(this);
    }

    [PunRPC]
    private void ExplodeRPC(float x, float y)
    {
        GameObject explosion = Instantiate(effects[0], new Vector3(x, y), Quaternion.identity);
        explosion.GetComponent<ParticleSystem>().Play();
    }

    public void Explode(Vector3 pos)
    {
        ExplodeRPC(pos.x, pos.y);
        photonView.RPC("ExplodeRPC", RpcTarget.Others, pos.x, pos.y);
    }

    [PunRPC]
    private void ExplodeEachPositionRPC(float _x, float _y, int range)
    {
        int playerPosX = (int)_x;
        int playerPosY = (int)_y;

        for (int x = playerPosX - range; x <= playerPosX + range; x++)
        {
            for (int y = playerPosY - range; y <= playerPosY + range; y++)
            {
                Vector3 pos = KeyBoard.instance.PositionWorldToKeyboard(new Vector3(x, y));

                if (pos.x >= 1 && pos.x <= KeyBoard.instance.width && pos.y >= 1 && pos.y <= KeyBoard.instance.height)
                {
                    GameObject explosion = Instantiate(effects[0], new Vector3(x, y), Quaternion.identity);
                    explosion.GetComponent<ParticleSystem>().Play();
                }
            }
        }
    }

    public void ExplodeEachPosition(Vector3 pos, int range)
    {
        ExplodeEachPositionRPC(pos.x, pos.y, range);
        photonView.RPC("ExplodeEachPositionRPC", RpcTarget.Others, pos.x, pos.y, range);
    }

    private IEnumerator ExplodeEachPositionDelay(object[] values)
    {
        float posX = (float)values[0];
        float posY = (float)values[1];
        int range = (int)values[2];
        float delay = (float)values[3];

        yield return new WaitForSeconds(delay);

        ExplodeEachPositionRPC(posX, posY, range);
        photonView.RPC("ExplodeEachPositionRPC", RpcTarget.Others, posX, posY, range);
    }

    public void ExplodeEachPosition(Vector3 pos, int range, float delay)
    {
        object[] values = new object[4] { pos.x, pos.y, range, delay };
        StartCoroutine("ExplodeEachPositionDelay", values);
    }

    [PunRPC]
    private void HealEffectRPC(float x, float y, int _heal)
    {
        GameObject heal = effects[1];
        heal.GetComponentInChildren<TextMeshProUGUI>().text = "+" + _heal.ToString();
        Instantiate(effects[1], new Vector3(x, y + .3f), Quaternion.identity);
    }

    public void HealEffect(Vector3 pos, int _heal)
    {
        HealEffectRPC(pos.x, pos.y, _heal);
        photonView.RPC("HealEffectRPC", RpcTarget.Others, pos.x, pos.y, _heal);
    }

    [PunRPC]
    private void ExplodeFeaderRPC(float x, float y)
    {
        GameObject explosion = Instantiate(effects[2], new Vector3(x, y + .3f), Quaternion.identity);
        explosion.GetComponent<ParticleSystem>().Play();
    }

    public void ExplodeFeader(Vector3 pos)
    {
        ExplodeFeaderRPC(pos.x, pos.y);
        photonView.RPC("ExplodeFeaderRPC", RpcTarget.Others, pos.x, pos.y);
    }

    [PunRPC]
    private void ExplodeFeaderWithBombRPC(float x, float y)
    {
        GameObject explosion = Instantiate(effects[3], new Vector3(x, y + .3f), Quaternion.identity);
        explosion.GetComponent<ParticleSystem>().Play();
    }

    public void ExplodeFeaderWithBomb(Vector3 pos)
    {
        ExplodeFeaderWithBombRPC(pos.x, pos.y);
        photonView.RPC("ExplodeFeaderWithBombRPC", RpcTarget.Others, pos.x, pos.y);
    }

    [PunRPC]
    private void DamageEffectRPC(float x, float y, int _damage)
    {
        GameObject damage = effects[4];
        damage.GetComponentInChildren<TextMeshProUGUI>().text = "-" + _damage.ToString();
        Instantiate(effects[4], new Vector3(x, y + .3f), Quaternion.identity);
    }

    public void DamageEffect(Vector3 pos, int damage)
    {
        DamageEffectRPC(pos.x, pos.y, damage);
        photonView.RPC("DamageEffectRPC", RpcTarget.Others, pos.x, pos.y, damage);
    }

    [PunRPC]
    private void DestroyEffectsRPC()
    {
        GameObject[] effects = GameObject.FindGameObjectsWithTag("Effect");

        foreach(GameObject effect in effects)
            Destroy(effect);
    }

    public void DestroyEffects()
    {
        DestroyEffectsRPC();
        photonView.RPC("DestroyEffectsRPC", RpcTarget.Others);
    }

    
}
