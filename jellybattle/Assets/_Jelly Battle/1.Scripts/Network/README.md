# Network(Photon Pun)
이 게임은 랜덤 매칭 방식입니다. 그래서 Photon Pun에서는 로비 생성을 지원하지만 바로 Room을 생성했습니다.
## 주요 코드
### [Launcher.cs]()
```c#
public void Connect()
{
    // 네트워크가 연결되고 준비가 되면
    if (PhotonNetwork.IsConnectedAndReady)
    {
        ...
        // 무작위 룸에 들어간다(랜덤 매칭)
        PhotonNetwork.JoinRandomRoom();
    }
    else
    {
        Debug.Log("Isn't ConnectedAndReady!!");
    }
}
```

```c#
public override void OnJoinRandomFailed(short returnCode, string message)
{
    ...
    // 랜덤으로 방을 찾지 못하면 방을 새로 생성
    PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
}

public override void OnPlayerEnteredRoom(Photon.Realtime.Player other)
{
    if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
    {
        // 방장이면
        if (PhotonNetwork.IsMasterClient)
        {
            // 방을 닫음
            PhotonNetwork.CurrentRoom.IsOpen = false;
            // 게임을 시작
            PhotonNetwork.LoadLevel(1);
        }
    }
}
```
위는 Photon Pun에서 제공하는 이벤트 함수들입니다. 랜덤으로 방을 찾지 못하면 방을 직접 생성합니다. 플레이어가 방에 들어오면 MasterClient가 LoadLevel하게 되어 모든 플레이어가 레벨을 로드하게 됩니다.

### RPC
---
### [Player.cs](https://github.com/ComeBiga/ChickenBattle_like_JellyBattle/blob/main/jellybattle/Assets/_Jelly%20Battle/1.Scripts/Player/Player.cs)
```c#
[PunRPC]
private void DamagedRPC(int damage, int type)
{
    currentHP -= damage;
}

public void Damaged(int damage, int type)
{
    ...
    photonView.RPC("DamagedRPC", RpcTarget.All, damage, type);
    ...
}
```
PunRPC 애트리뷰트가 달린 함수는 photonView.RPC 함수로 호출될 수가 있고 RpcTarget.All 해주면 자신을 포함한 모든 플레이어의 클라이언트에서 해당 함수를 실행하게 된다.
