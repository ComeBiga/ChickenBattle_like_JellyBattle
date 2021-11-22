using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerLocal : MonoBehaviour
{
    #region Singleton

    public static AudioManagerLocal instance = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    #endregion

    [PunRPC]
    private void PlayRPC(string name)
    {
        AudioManager.instance.Play(name);
    }

    public void Play(string name)
    {
        PlayRPC(name);
        PhotonView pv = PhotonView.Get(this);
        pv.RPC("PlayRPC", RpcTarget.Others, name);
    }

    private IEnumerator PlayDelay(object[] values)
    {
        string name = (string)values[0];
        float delay = (float)values[1];

        yield return new WaitForSeconds(delay);

        PlayRPC(name);
        PhotonView pv = PhotonView.Get(this);
        pv.RPC("PlayRPC", RpcTarget.Others, name);
    }

    public void Play(string name, float delay)
    {
        object[] values = new object[2] { name, delay };

        StartCoroutine("PlayDelay", values);
    }

    [PunRPC]
    private void PlayDifferentWithNickNameRPC(string one, string two, int actNum)
    {
        if(PhotonNetwork.LocalPlayer.ActorNumber == actNum)
        {
            AudioManager.instance.Play(one);
        }
        else
        {
            AudioManager.instance.Play(two);
        }
    }

    public void PlayDifferentWithNickName(string one, string two, int actNum)
    {
        PlayDifferentWithNickNameRPC(one, two, actNum);
        PhotonView pv = PhotonView.Get(this);
        pv.RPC("PlayDifferentWithNickNameRPC", RpcTarget.Others, one, two, actNum);
    }
}
