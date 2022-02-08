# CheckSamePosition
![겹치는케이스1_1](https://user-images.githubusercontent.com/36800639/153059250-894f46a0-8be5-4448-a4af-71a29fffd698.png)
![겹치는케이스2_2](https://user-images.githubusercontent.com/36800639/153060132-17cab16a-9183-4b31-b957-83c3a04ed61f.png)

두 플레이어가 같은 칸을 선택한 경우에 잠시동안 같은 칸에 위치합니다. 잠시지만 그 전 위치에 따라서 캐릭터가 다르게 배치가 됩니다.
## 해결 방법
![겹치기전_그래프방정식](https://user-images.githubusercontent.com/36800639/153059525-ebb57f27-7d2e-4b81-ad1f-d385761d25a2.png)
![겹치는장면_그래프](https://user-images.githubusercontent.com/36800639/153059773-7afeab5b-0bf6-44bf-befb-5112983e6502.png)

겹치는 칸을 교점으로 두 대각선을 그었을 때, 대각선을 기준으로 하는 위치에 따라서 캐릭터를 배치합니다.

## 주요 코드
### [CheckSamePosition.cs](https://github.com/ComeBiga/ChickenBattle_like_JellyBattle/blob/main/jellybattle/Assets/_Jelly%20Battle/1.Scripts/Managers/CheckSamePosition.cs)
```c#
public void LocateOverlapedPlayers()
{
    ...

    // 목표 위치의 y절편 값
    int bPositive = (int)(nextPos.y - nextPos.x);
    int bNegative = (int)(nextPos.y + nextPos.x);

    foreach(Player player in ol)
    {
        // 플레이어 위치의 y절편 값
        Vector3 playerPos = player.transform.position;
        int yPositive = (int)(playerPos.y - playerPos.x);
        int yNegative = (int)(playerPos.y + playerPos.x);

        // Top
        if (yPositive >= bPositive && yNegative >= bNegative)
            locations[0].Add(player);
        // left
        else if (yPositive >= bPositive && yNegative < bNegative)
            locations[1].Add(player);
        // right
        else if (yPositive < bPositive && yNegative >= bNegative)
            locations[2].Add(player);
        // bottom
        else if (yPositive < bPositive && yNegative < bNegative)
            locations[3].Add(player);
    }

    ...
}
```

```c#

```
