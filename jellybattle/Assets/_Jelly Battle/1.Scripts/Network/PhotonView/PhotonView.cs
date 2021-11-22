using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChickenHead
{
    public class PhotonView
    {
        public static void RPC(Photon.Pun.PhotonView photon, string methodName, params object[] parameters)
        {
            
            photon.RPC(methodName, Photon.Pun.RpcTarget.Others, parameters);
        }
    }
}
