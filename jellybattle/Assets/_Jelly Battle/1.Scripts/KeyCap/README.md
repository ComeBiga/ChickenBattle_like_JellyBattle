# Item(KeyCap.cs)
## 구조
![Kc](https://user-images.githubusercontent.com/36800639/153057137-54efa9dd-4560-4014-a46b-ea02263281f9.png)

아이템은 구현은 다양해지지만 사용방식은 같기 때문에 위와 같은 구조로 디자인했습니다.
## [전체 코드]()
## 주요 코드
### KeyCap.cs
```c#
public abstract class KeyCap : MonoBehaviour
{
    ...
    public abstract void Use(Player player, Player[] enemies);
    ...
}
```

### BoltKeyCap.cs
```c#
public class BoltKeyCap : KeyCap
{
    public override void Use(Player player, Player[] enemies)
    {
        ...
    }
}
```

### Player.cs
```c#
public void UsingState(Player player, Player[] enemies)
{
    if (keyCaps.Current.isActive)
    {
        keyCaps.Current.Use(player, enemies);

        ...
    }

    ...
}
```
